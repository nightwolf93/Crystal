using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Interop.PythonScripting;
using Crystal.WorldServer.Engines.Spells;

namespace Crystal.WorldServer.World.Game.Fights
{
    public class Fighter
    {
        #region Fields

        public Network.WorldClient Client { get; set; }
        public Database.Records.MonsterLevelRecord Monster { get; set; }
        public FightTeam Team { get; set; }
        public bool IsHuman { get; set; }
        public bool IsDead { get; set; }
        public bool IsReady { get; set; }
        public bool IsInvoc { get; set; }

        public int SummonOwner = -1;

        public int StartLife { get; set; }

        /* Fight location */
        public int CellID { get; set; }
        public int Dir { get; set; }
        public int NextCell { get; set; }
        public int NextDir { get; set; }

        public int CurrentAP { get; set; }
        public int CurrentMP { get; set; }
        public Dictionary<int, FightSpellCooldown> Cooldowns = new Dictionary<int, FightSpellCooldown>();

        public FightDrops Drops { get; set; }

        /* Objects */
        public List<Engines.Spells.SpellBuff> Buffs = new List<Engines.Spells.SpellBuff>();
        public List<FightShield> Armors = new List<FightShield>();
        public List<FightTrap> OwnGlyph = new List<FightTrap>();
        public List<FighterState> States = new List<FighterState>();

        /* Wear effects */
        public Fighter WearedFighter = null;
        public Fighter ByWearFighter = null;

        /* Shortcuts */
        public Database.Records.CharacterRecord Character { get { return Client.Character; } }
        public Engines.Map.MonsterGroup MonsterGroup { get; set; }

        /* Monsters */
        private int _id;
        private int _currentLife;

        /* Challenges fields */
        public int CurrentUsedPmThisTurn = 0;

        public AI.MonsterAI ArtificialBrain { get; set; }

        #endregion

        #region Builders

        public Fighter(Network.WorldClient client)
        {
            this.Drops = new FightDrops(this);
            this.Client = client;
            this.Client.Character.Fighter = this;
            this.IsHuman = true;
            this.IsReady = false;
            this.IsDead = false;
            this.StartLife = client.Character.CurrentLife;
            this.CurrentAP = client.Character.Stats.GetMaxActionPoints;
            this.CurrentMP = client.Character.Stats.GetMaxMovementPoints;
            this.Dir = client.Character.Direction;
            this.CellID = client.Character.CellID;
            this.NextCell = -1;
            this.NextDir = -1;
        }

        public Fighter(int id, Database.Records.MonsterLevelRecord monster, Engines.Map.MonsterGroup group)
        {
            this.Drops = new FightDrops(this);
            this.MonsterGroup = group;
            this.Monster = monster;

            this._id = id;
            this._currentLife = this.Monster.Life;

            this.IsHuman = false;
            this.IsReady = true;
            this.IsDead = false;

            switch (Monster.GetTemplate.AI)
            {
                case 0:
                    this.ArtificialBrain = new AI.StaticAI(this);
                    break;

                case 1:
                case 2:
                    this.ArtificialBrain = new AI.BasicAI(this);
                    break;

                case 3:
                    this.ArtificialBrain = new AI.FearfulAI(this);
                    break;

                case 4:
                    this.ArtificialBrain = new AI.ScriptedAI(this);
                    if (this.Monster.GetTemplate.HasScriptAI())
                    {
                        var scriptID = this.Monster.GetTemplate.Script;
                        var script = new PyScript("Scripts/AI/" + scriptID);
                        script.Load(this);

                        (this.ArtificialBrain as AI.ScriptedAI).Script = script;
                    }
                    break;
            }

            this.CurrentAP = Stats.GetMaxActionPoints;
            this.CurrentMP = Stats.GetMaxMovementPoints;
        }

        #endregion

        #region Methods

        public void Send(string packet)
        {
            if (IsHuman)
            {
                Client.Send(packet);
            }
        }

        public void ResetPoints()
        {
            this.CurrentAP = this.Stats.GetMaxActionPoints;
            this.CurrentMP = this.Stats.GetMaxMovementPoints;

            this.Stats.RefreshStats();
        }

        public bool HasReverseDamagesLuck()
        {
            return this.Buffs.FindAll(x => x is Spells.Buffs.AddLuckEcaflipBuff).Count > 0;
        }

        public void TakeDamages(int caster, int damages, int type, bool returned = false)
        {
            //FIX OVERFLOW DAMAGES in 1.2.0.1
            if (IsDead) return;
            int baseDamages = damages;

            if (!this.IsHuman)
            {
                if (this.ArtificialBrain is AI.ScriptedAI)
                {
                    damages = ((Fights.AI.ScriptedAI)this.ArtificialBrain).Script.CallTakingDamages(this, damages);
                }
            }

            //REVERSE DAMAGES in 1.0.4.2
            var reversedDamages = this.GetReversedDamages(true);
            if (!returned)
            {                
                damages += reversedDamages;
                if (damages > 0) damages = 0;
            }

            damages = GetReducedDamages(damages, type, true);//1.0.4.3 - Armor

            if (this.HasReverseDamagesLuck())//Ecaflip luck spell 1.0.4.3
            {
                if (Utilities.Basic.Rand(0, 2) == 0)//take damages
                {
                    damages *= 2;
                }
                else
                {
                    damages = -damages;
                }
            }

            var nextLife = CurrentLife + damages;
            if (nextLife <= 0)
            {
                Team.Fight.Send("GA;100;" + caster + ";" + this.ID + "," + (-CurrentLife));
            }
            else
            {
                Team.Fight.Send("GA;100;" + caster + ";" + this.ID + "," + damages);
            }

            CurrentLife += damages;

            if (CurrentLife <= 0)//FIXED 0 life points in 1.2.0.1
            {
                CurrentLife = 0;
                this.Team.Fight.CheckMonsterKilledChallenge(this.Team.Fight.GetFighter(caster), this);
                EnableDeadState();

                if (this.Team.Fight.TimeLine != null)
                {
                    if (this.Team.Fight.TimeLine.CurrentFighter != null)
                    {
                        if (this.Team.Fight.TimeLine.CurrentFighter.ID == this.ID)//Auto kill ? Pass turn in the timeline FIXED in 1.2.0.1
                        {
                            this.Team.Fight.FinishTurnRequest(this);
                        }
                    }
                }
            }
            else
            {
                foreach (Engines.Spells.SpellBuff buff in Buffs.ToArray())
                {
                    buff.FighterHit(damages);
                }
            }

            if (!returned)
            {
                if (reversedDamages > 0)
                {
                    try
                    {
                        if (reversedDamages > -(baseDamages)) reversedDamages = -baseDamages;
                        var casterPlayer = this.Team.Fight.GetFighter(caster);
                        if (casterPlayer != null)
                        {
                            casterPlayer.TakeDamages(this.ID, -(reversedDamages), 1, true);
                        }
                    }
                    catch (Exception e)
                    {
                        Utilities.ConsoleStyle.Infos("Can't reverse damages : " + e.ToString());
                    }
                }
            }
        }

        public void Heal(int caster, int healPoint, int type)
        {
            if (IsDead) return;

            if (CurrentLife + healPoint > Stats.MaxLife)
            {
                healPoint = Stats.MaxLife - CurrentLife;
            }
            if (healPoint < 0)
            {
                healPoint = 0;
            }
            Team.Fight.Send("GA;100;" + caster +";" + ID + "," + healPoint);
            CurrentLife += healPoint;
            this.Team.Fight.CheckHealChallenge(this.Team.Fight.GetFighter(caster), this);
        }

        public void EnableDeadState()
        {
            try
            {
                this.IsDead = true;
                if (Team.Fight.TimeLine.CurrentFighter != null)
                {
                    Team.Fight.Send("GA;103;" + Team.Fight.TimeLine.CurrentFighter.ID + ";" + ID);
                }
                this.RemoveAllSubEntities();
                Team.Fight.CheckEnd();
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant execute dead state because : " + e.ToString());
            }
        }

        public void RemoveAllSubEntities()
        {
            this.KillAllSummonedInvocs();
            foreach (FightTrap glyph in this.OwnGlyph.ToArray())
            {
                glyph.RemoveGlyph();
            }
        }

        public void AddBuff(int caster, Engines.Spells.SpellBuff buff, int spriteID, int value, bool sendPacket, Engines.Spells.SpellEffect effect)
        {
            this.Buffs.Add(buff);
            buff.ApplyBuff();
            
            if (sendPacket)
            {
                this.Team.Fight.Send("GA;" + spriteID + ";" + caster + ";" + ID + "," + value + "," + buff.Duration);

                string packetShowedBuffInStatusBar = "GIE" + (int)effect.Effect;
                packetShowedBuffInStatusBar += ";" + this.ID;
                packetShowedBuffInStatusBar += ";" + effect.Value.ToString();
                packetShowedBuffInStatusBar += ";" + (effect.Value2.ToString() != "-1" ? effect.Value2.ToString() : "").ToString();
                packetShowedBuffInStatusBar += ";" + (effect.Value3.ToString() != "-1" ? effect.Value3.ToString() : "").ToString();
                packetShowedBuffInStatusBar += ";" + (effect.Chance.ToString() != "-1" ? effect.Chance.ToString() : "").ToString();
                packetShowedBuffInStatusBar += ";" + (effect.Turn > 0 ? effect.Turn.ToString() : "").ToString();
                packetShowedBuffInStatusBar += ";" + effect.Engine.Spell.ID.ToString();

                this.Team.Fight.Send(packetShowedBuffInStatusBar);
            }
        }

        public void CheckSpellTurnAndBuff()
        {
            try
            {
                this.CurrentUsedPmThisTurn = 0;
                RefreshChatiments();
                foreach (Engines.Spells.SpellBuff buff in this.Buffs.ToArray())
                {
                    buff.RemovedOneTurDuration();
                    if (buff.Duration < 0)
                    {
                        buff.BuffRemoved();
                        this.Buffs.Remove(buff);
                        this.Stats.RefreshStats();
                    }
                }
                foreach (FightTrap glyph in this.OwnGlyph.ToArray())
                {
                    glyph.RemoveOneTurnDuration();
                }
                foreach (var cooldown in this.Cooldowns.ToArray())
                {
                    cooldown.Value.Remove();
                    if (cooldown.Value.Time <= 0)
                    {
                        this.Cooldowns.Remove(cooldown.Key);
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant remove turn buff : " + e.ToString());
            }
        }

        public void CheckWalkingGlyph()
        {
            foreach (FightTrap glyph in this.Team.Fight.Glyphs)
            {
                if (glyph.WalkOnTrap(this.CellID))
                {
                    glyph.UseEffect(this.CellID, this);
                }
            }
        }

        public List<Fighter> GetSummonedInvocs()
        {
            return Team.Fighters.FindAll(x => x.SummonOwner == this.ID);
        }

        public void KillAllSummonedInvocs()
        {
            foreach (Fighter summonedFighter in GetSummonedInvocs())
            {
                summonedFighter.EnableDeadState();
            }
        }

        public void RemoveState(FighterState state)
        {
            States.Remove(state);
            foreach (Engines.Spells.SpellBuff buff in this.Buffs.ToArray())
            {
                if (buff.StateType == state)
                {
                    buff.BuffRemoved();
                    this.Buffs.Remove(buff);
                }
            }
        }

        public void RemoveAllBuffs()
        {
            foreach (Engines.Spells.SpellBuff buff in this.Buffs.ToArray())
            {
                buff.BuffRemoved();
                this.Buffs.Remove(buff);
            }
            this.Stats.RefreshStats();
        }

        public void Wear(Fighter weared)
        {
            this.WearedFighter = weared;
            weared.ByWearFighter = this;

            weared.CellID = this.CellID;

            this.Team.Fight.Send("GA;950;" + this.ID + ";" + this.ID + "," + (int)FighterState.Porteur + ",1");
            this.Team.Fight.Send("GA;950;" + weared.ID + ";" + weared.ID + "," + (int)FighterState.Porte + ",1");

            this.Team.Fight.Send("GA0;50;" + this.ID + ";" + weared.ID);
        }

        public void UnWear()
        {
            if (ByWearFighter != null)
            {
                this.Team.Fight.Send("GA;950;" + this.ID + ";" + this.ID + "," + (int)FighterState.Porte + ",0");
                this.Team.Fight.Send("GA;950;" + ByWearFighter.ID + ";" + ByWearFighter.ID + "," + (int)FighterState.Porteur + ",0"); 
                ByWearFighter.WearedFighter = null;
                ByWearFighter = null;
            }
        }

        public void UnInvisible()
        {
            if (this.States.Contains(FighterState.Invisible))
            {
                this.States.Remove(Fights.FighterState.Invisible);
                this.Team.Fight.Send("GA;150;" + this.ID + ";" + this.ID);
                this.Team.Fight.Send("GA;4;" + this.ID + ";" + this.ID + "," + this.CellID);   
            }  
        }

        public void RefreshChatiments()
        {
            try
            {
                foreach (Spells.Buffs.AddChatiment chatiment in this.Buffs.FindAll(x => x.GetType() == typeof(Spells.Buffs.AddChatiment)))
                {
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't check chatiment : " + e.ToString());
            }
        }

        public bool CanCastSpell(int id)
        {
            return !this.Cooldowns.ContainsKey(id);
        }

        public int GetReversedDamages(bool sendPacket = false)
        {
            int value = 0;
            foreach (var b in this.Buffs)
            {
                if (b is Spells.Buffs.AddReverseDamage)
                {
                    value += ((Spells.Buffs.AddReverseDamage)b).CalculedReversedDamages;
                }
            }
            value += this.Stats.ReverseDamagesBonus.Total;

            if (value != 0 && sendPacket)
            {
                this.Team.Fight.Send("GA;107;-1;" + this.ID + "," + value);
            }

            return value;
        }

        public int GetReducedDamages(int damages, int type, bool sendPacket = false)
        {
            var nextDamages = damages;
            var armor = GetFixArmorReduce();
            nextDamages += armor;

            if (nextDamages >= 0) nextDamages = 0;

            if (sendPacket && nextDamages != damages)
            {
                this.Team.Fight.Send("GA;105;-1;" + this.ID + "," + armor);
            }

            return nextDamages;
        }

        public int GetFixArmorReduce()
        {
            var armor = 0;
            this.Buffs.FindAll(x => x is Spells.Buffs.AddArmorBuff).ForEach(x => armor += (x as Spells.Buffs.AddArmorBuff).Value);
            return armor;
        }

        public Spells.Buffs.AddReverseSpellBuff HasReverseSpellBuff()
        {
            Spells.Buffs.AddReverseSpellBuff b = null;
            foreach (var buff in this.Buffs.FindAll(x => x is Spells.Buffs.AddReverseSpellBuff))
            {
                if (b != null)
                {
                    if ((buff as Spells.Buffs.AddReverseSpellBuff).Level > b.Level)
                    {
                        b = buff as Spells.Buffs.AddReverseSpellBuff;
                    }
                }
                else
                {
                    b = buff as Spells.Buffs.AddReverseSpellBuff;
                }
            }
            return b;
        }

        #endregion

        #region Generic Fields

        public int ID
        {
            get
            {
                if (IsHuman)
                {
                    return Character.ID;
                }
                else
                {
                    return _id;
                }
            }
        }

        public int Initiative
        {
            get
            {
                if (this.IsHuman)
                {
                    return this.Character.Stats.Initiative;
                }
                else
                {
                    return this.Monster.Life * 2;
                }
            }
        }

        public virtual string DisplayPattern
        {
            get
            {
                if (this.IsHuman)
                {
                    return this.Client.Character.Pattern.ShowCharacterInBattle;
                }
                else
                {
                    string packet = this.CellID + ";1;0;" + this.ID + ";" + this.Monster.TemplateID
                         + ";-2;" + this.Monster.GetTemplate.Skin + "^" + this.Monster.Size + ";" + this.Monster.Level + ";"
                             + this.Monster.GetTemplate.Color1.ToString("x") + ";" + this.Monster.GetTemplate.Color2.ToString("x") + ";"
                             + this.Monster.GetTemplate.Color3.ToString("x") + ";0,0,0,0;" + Stats.MaxLife + ";" + CurrentAP + ";" + CurrentMP
                             + ";0;0;0;0;0;0;0;" + Team.ID;
                    return packet;
                }
            }
        }

        public int MapCell
        {
            get
            {
                if (this.IsHuman)
                {
                    return this.Client.Character.CellID;
                }
                else
                {
                    return this.MonsterGroup.CellID;
                }
            }
        }

        public Engines.StatsEngine Stats
        {
            get
            {
                if (IsHuman)
                {
                    return Character.Stats;
                }
                else
                {
                    return Monster.StatsEngine;
                }
            }
        }

        public int CurrentLife
        {
            get
            {
                if (IsHuman)
                {
                    return this.Character.CurrentLife;
                }
                else
                {
                    return this._currentLife;
                }
            }
            set
            {
                if (IsHuman)
                {
                    this.Character.CurrentLife = value;
                }
                else
                {
                    this._currentLife = value;
                }
            }
        }

        public string Nickname
        {
            get
            {
                if (IsHuman)
                {
                    return this.Character.Nickname;
                }
                else
                {
                    return this.Monster.GetTemplate.Name;
                }
            }
        }

        public int Level
        {
            get
            {
                if (IsHuman)
                {
                    return this.Character.Level;
                }
                else
                {
                    return this.Monster.Level;
                }
            }
        }

        public int Look
        {
            get
            {
                if (IsHuman)
                {
                    return Character.Look;
                }
                else
                {
                    return Monster.GetTemplate.Skin;
                }
            }
        }

        public int LifePercentage
        {
            get
            {
                if (!IsHuman)
                {
                    float percentage = ((float)CurrentLife / (float)this.Monster.Life);
                    return (int)(percentage * 100);
                }
                else
                {
                    float percentage = ((float)CurrentLife / (float)this.Stats.MaxLife);
                    return (int)(percentage * 100);
                }
            }
        }

        public AI.MonsterAI AI
        {
            get
            {
                return this.ArtificialBrain;
            }
        }

        public Fight GetFight()
        {
            return this.Team.Fight;
        }

        #endregion
    }
}
