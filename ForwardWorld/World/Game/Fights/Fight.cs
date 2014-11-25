using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Fights
{
    public class Fight
    {
        public int ID;
        public Enums.FightTypeEnum FightType = Enums.FightTypeEnum.Challenge;
        public FightState State = FightState.PlacementsPhase;
        public FightTimeline TimeLine { get; set; }
        public bool Alive = true;

        public long StartTime = 0;
        public bool BlockSpectator = false;
        public System.Timers.Timer StartTimer { get; set; }
        public System.Timers.Timer CheckTimer { get; set; }

        /* Teams */
        public FightTeam RedTeam { get; set; }
        public FightTeam BlueTeam { get; set; }

        public List<FightSpectator> Spectators = new List<FightSpectator>();
        public Engines.MapEngine Map { get; set; }

        public int CurrentEntityTempID = -10;

        /* Fights objects */
        public List<FightTrap> Traps = new List<FightTrap>();
        public List<FightTrap> Glyphs = new List<FightTrap>();
        public List<Challenges.FightChallenge> Challenges = new List<Challenges.FightChallenge>();

        /// <summary>
        /// Init challenge player vs player
        /// </summary>
        public Fight(Fighter fighter1, Fighter fighter2, Engines.MapEngine map, Enums.FightTypeEnum fightType)
        {
            this.FightType = fightType;
            this.Map = map;
            this.ID = this.Map.GetActorAvailableID;
            this.InitTeam(fighter1, fighter2);
            this.InitPlaces();
            this.InitTimeline();
            this.PlacePlayer(fighter1);
            this.PlacePlayer(fighter2);
            this.StartTime = Environment.TickCount;
            this.InitCheckFight();

            if (FightType == Enums.FightTypeEnum.Agression)
            {
                this.StartTimer = new System.Timers.Timer(30000);
                this.StartTimer.Enabled = true;
                this.StartTimer.Elapsed += new System.Timers.ElapsedEventHandler(StartTimer_Elapsed);
                this.StartTimer.Start();
            }
        }

        /// <summary>
        /// Init fight with mobs
        /// </summary>
        public Fight(Fighter fighter1, Engines.Map.MonsterGroup monsters, Engines.MapEngine map,
                                                        Enums.FightTypeEnum fightType = Enums.FightTypeEnum.PvM)
        {
            //Stop the bonus timer
            monsters.StopBonusTimer();

            this.FightType = fightType;
            this.Map = map;
            this.ID = this.Map.GetActorAvailableID;
            this.RedTeam = new FightTeam(0, fighter1, this, false);
            this.BlueTeam = new FightTeam(1, new Fighter(-9, monsters.Leader, monsters), this, true);
            this.InitPlaces();
            this.InitMontersGroup(monsters);
            this.InitTimeline();
            this.PlacePlayer(fighter1);
            this.StartTime = Environment.TickCount;
            this.InitCheckFight();
            if (FightType == Enums.FightTypeEnum.PvM)
            {
                this.StartTimer = new System.Timers.Timer(30000);
                this.StartTimer.Enabled = true;
                this.StartTimer.Elapsed += new System.Timers.ElapsedEventHandler(StartTimer_Elapsed);
                this.StartTimer.Start();
            }
        }

        #region Init

        public void InitTeam(Fighter fighter1, Fighter fighter2)
        {
            this.RedTeam = new FightTeam(0, fighter1, this, false);
            this.BlueTeam = new FightTeam(1, fighter2, this, false);
        }

        public void InitTimeline()
        {
            this.TimeLine = new FightTimeline(this);
            this.TimeLine.RemixTimeLine();
        }

        public void InitMontersGroup(Engines.Map.MonsterGroup group)
        {
            foreach (Database.Records.MonsterLevelRecord monster in group.Monsters)
            {
                Fighter monsterFighter = new Fighter(this.CurrentEntityTempID, monster, group);
                this.BlueTeam.AddToTeam(monsterFighter);
                this.PlacePlayer(monsterFighter);
                this.CurrentEntityTempID--;
            }
        }

        public void PlacePlayer(Fighter fighter)
        {
            fighter.CellID = RandomAvailableTeamPlaces(fighter.Team);
        }

        public void InitPlaces()
        {
            int timeout = 0;
            if (this.Map.Places.Count > 0)
            {
                foreach (int redplace in this.Map.Places[0])
                {
                    this.RedTeam.PlacementsPlaces.Add(redplace);
                }
                foreach (int blueplace in this.Map.Places[1])
                {
                    this.BlueTeam.PlacementsPlaces.Add(blueplace);
                }
            }
            else
            {
                for (int i = 0; i <= Utilities.ConfigurationManager.GetIntValue("MaxPlacementsPlacesPerTeam") - 1; i++)
                {
                    int randomRedCell = this.Map.RandomFreeCell().ID;
                    while (this.RedTeam.PlacementsPlaces.Contains(randomRedCell))
                    {
                        randomRedCell = this.Map.RandomFreeCell().ID;
                        timeout++;
                        if (timeout > 100)
                            break;
                    }
                    this.RedTeam.PlacementsPlaces.Add(randomRedCell);
                }
                for (int i = 0; i <= Utilities.ConfigurationManager.GetIntValue("MaxPlacementsPlacesPerTeam") - 1; i++)
                {
                    int randomBlueCell = this.Map.RandomFreeCell().ID;
                    while (this.RedTeam.PlacementsPlaces.Contains(randomBlueCell) && this.BlueTeam.PlacementsPlaces.Contains(randomBlueCell))
                    {
                        randomBlueCell = this.Map.RandomFreeCell().ID;
                        timeout++;
                        if (timeout > 100)
                            break;
                    }
                    this.BlueTeam.PlacementsPlaces.Add(randomBlueCell);
                }
            }
        }

        public void InitCheckFight()
        {
            CheckTimer = new System.Timers.Timer(1000);
            CheckTimer.Enabled = true;
            CheckTimer.Elapsed += CheckTimer_Elapsed;
            CheckTimer.Start();
        }

        private void CheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.State == FightState.PlacementsPhase)
            {
                this.CheckEnd();
            }
            if (this.State == FightState.Fighting)
            {
                this.CheckEnd();
            }
        }

        public void EnableContext()
        {
            this.Fighters.ForEach(x => ShowFightToClient(x));
        }

        public void ShowFightToClient(Fighter fighter)
        {
            this.Send("GM|+" + fighter.DisplayPattern);
            switch (this.FightType)
            {
                case Enums.FightTypeEnum.Challenge:
                    fighter.Send("GJK2|1|1|0|0|0");
                    break;

                case Enums.FightTypeEnum.PvM:
                    fighter.Send("GJK2|0|1|0|29999|4");
                    break;

                case Enums.FightTypeEnum.Kolizeum:
                case Enums.FightTypeEnum.Agression:
                    fighter.Send("GJK2|0|1|0|29999|1");
                    break;
            }
            fighter.Send("GP" + this.DisplayPatternPlace + "|" + fighter.Team.ID);
            fighter.Send("GM" + this.DisplayPatternFighters);
        }

        public void ChangePlayerPlace(Fighter fighter, int cellID)
        {
            if (fighter.Team.PlacementsPlaces.Contains(cellID))
            {
                if (!fighter.Team.HaveFightersOnThisPlace(cellID) && !fighter.IsReady)
                {
                    fighter.CellID = cellID;
                    this.Send("GIC|" + fighter.ID + ";" + cellID + ";1");
                }
                else
                {
                    fighter.Send("BN");
                }
            }
        }

        public void ChangeReadyState(Fighter fighter, int state)
        {
            if (this.State == FightState.PlacementsPhase)
            {
                if (state == 0)// Not ready
                {
                    fighter.IsReady = false;
                    this.Send("GR0" + fighter.ID);
                }
                else if (state == 1)// Ready for fight
                {
                    fighter.IsReady = true;
                    this.Send("GR1" + fighter.ID);
                }
                this.TryStartFight();
            }
        }

        public void AddPlayer(Fighter fighter, int teamID, int cellID = -1)
        {
            FightTeam team = teamID == 0 ? RedTeam : BlueTeam;
            team.AddFighter(fighter);
            if(cellID == -1)
                this.PlacePlayer(fighter);
            this.ShowFightToClient(fighter);
        }

        public void AddSpectator(FightSpectator spectator)
        {
            this.Spectators.Add(spectator);
            switch (this.FightType)
            {
                case Enums.FightTypeEnum.Challenge:
                    spectator.Send("GJK2|1|1|0|0|0");
                    break;

                case Enums.FightTypeEnum.PvM:
                    spectator.Send("GJK2|0|1|0|29999|4");
                    break;

                case Enums.FightTypeEnum.Agression:
                    spectator.Send("GJK2|0|1|0|29999|1");
                    break;
            }
            spectator.Send("GM" + this.DisplayPatternFighters);
            spectator.Send("GIC" + DisplayPatternTimelinePos);
            spectator.Send("GS");
            spectator.Send("GTL" + DisplayPatternFighterID);
            this.WarnNewSpectator(spectator);
        }

        private void WarnNewSpectator(FightSpectator spectator)
        {
            try
            {
                this.Fighters.ForEach(x => x.Client.Action.SystemMessage("Le joueur <b>" + spectator.Client.Character.Nickname + "</b> vien de rejoindre le combat en spectateur !"));
                this.Spectators.ForEach(x => x.Client.Action.SystemMessage("Le joueur <b>" + spectator.Client.Character.Nickname + "</b> vien de rejoindre le combat en spectateur !"));
            }
            catch (Exception e) { }
        }

        public void TryStartFight(bool force = false)
        {
            if (IsAllReady && IsAvailableBattle() || force)
            {
                if (StartTimer != null)
                {
                    this.StartTimer.Close();
                    this.StartTimer.Stop();
                    this.StartTimer.Enabled = false;
                }
                this.Map.HideFight(this);
                this.State = FightState.Fighting;
                this.TimeLine.RemixTimeLine();
                this.Send("GS");
                this.TimelineDisplay();
                this.TimeLine.StartTimelineTasks();
                this.InitChallenges();
            }
        }

        public void TimelineDisplay()
        {
            this.Send("GIC" + DisplayPatternTimelinePos);
            this.Send("GTL" + DisplayPatternFighterID);
        }

        private void StartTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.StartTimer.Close();
            this.StartTimer.Stop();
            if (StartTimer.Enabled)
            {
                TryStartFight(true);
            }
            this.StartTimer.Enabled = false;
        }

        #endregion

        #region Getters

        public int RandomAvailableTeamPlaces(FightTeam team)
        {
            int randomCell = team.PlacementsPlaces[Utilities.Basic.Rand(0, team.PlacementsPlaces.Count - 1)];
            int timeout = 0;
            while (team.HaveFightersOnThisPlace(randomCell))
            {
                randomCell = team.PlacementsPlaces[Utilities.Basic.Rand(0, team.PlacementsPlaces.Count - 1)];
                timeout++;
                if (timeout > 100)
                    break;
            }
            return randomCell;
        }

        public string DisplayPatternPlace
        {
            get
            {
                string places = ("");
                foreach (int cell in RedTeam.PlacementsPlaces)
                {
                    places += Engines.Pathfinding.GetCellChars(cell);
                }
                places += "|";
                foreach (int cell in BlueTeam.PlacementsPlaces)
                {
                    places += Engines.Pathfinding.GetCellChars(cell);
                }
                return places;
            }
        }

        public string DisplayPatternFighters
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                this.Fighters.ForEach(x => pattern.Append("|+").Append(x.DisplayPattern));
                return pattern.ToString();
            }
        }

        public string DisplayPatternBlades
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                pattern.Append(this.ID.ToString())
                    .Append(";")
                    .Append(((int)this.FightType).ToString())
                    .Append("|")
                    .Append(this.RedTeam.ID.ToString())
                    .Append(";")
                    .Append(this.RedTeam.BladeCell.ToString())
                    .Append(";")
                    .Append(this.RedTeam.VirtualTeam ? "1" : "0")
                    .Append(";")
                    .Append(FightType == Enums.FightTypeEnum.Agression ? this.RedTeam.Leader.Character.FactionID.ToString() : "-1")//Alignement
                    .Append("|")
                    .Append(this.BlueTeam.ID.ToString())
                    .Append(";")
                    .Append(this.BlueTeam.BladeCell.ToString())
                    .Append(";")
                    .Append(this.BlueTeam.VirtualTeam ? "1" : "0")
                    .Append(";")
                    .Append(FightType == Enums.FightTypeEnum.Agression ? this.BlueTeam.Leader.Character.FactionID.ToString() : "-1");//Alignement

                return pattern.ToString();
            }
        }

        public string DisplayPatternTimelinePos
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                this.TimeLine.TimeLine.ForEach(x => pattern.Append("|" + x.ID + ";" + x.CellID + ";1"));
                return pattern.ToString();
            }
        }

        public string DisplayPatternFighterID
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                this.TimeLine.TimeLine.ForEach(x => pattern.Append("|" + x.ID));
                return pattern.ToString();
            }
        }

        public string DisplayFightInformations
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                pattern.Append(this.ID.ToString())
                    .Append(";0;0,");//TODO: Time
                switch (this.FightType)
                {
                    case Enums.FightTypeEnum.Challenge:
                        pattern.Append("0,")
                            .Append(this.BlueTeam.Fighters.Count.ToString())
                            .Append(";")
                            .Append("0,")
                            .Append("0,")
                            .Append(this.RedTeam.Fighters.Count.ToString())
                            .Append(";");
                        break;
                }
                return pattern.ToString();
            }
        }

        public string DisplayFightDetails
        {
            get
            {
                StringBuilder pattern = new StringBuilder();
                pattern.Append(this.ID.ToString())
                    .Append("|");//TODO: Time
                this.BlueTeam.Fighters.ForEach(x => pattern.Append(x.Nickname).Append("~").Append(x.Level).Append(";"));
                pattern.Append("|");
                this.RedTeam.Fighters.ForEach(x => pattern.Append(x.Nickname).Append("~").Append(x.Level).Append(";"));
                return pattern.ToString();
            }
        }

        public List<Fighter> Fighters
        {
            get
            {
                List<Fighter> fighters = new List<Fighter>();
                fighters.AddRange(this.RedTeam.Fighters);
                fighters.AddRange(this.BlueTeam.Fighters);
                return fighters;
            }
        }

        public bool IsAllReady
        {
            get
            {
                return this.Fighters.FindAll(x => x.IsReady == false).Count == 0;
            }
        }

        public Fighter GetFighterOnCell(int cellID)
        {
            if (Fighters.FindAll(x => x.CellID == cellID && !x.IsDead).Count > 0)
            {
                return Fighters.FirstOrDefault(x => x.CellID == cellID && !x.IsDead);
            }
            return null;
        }

        public Fighter GetFighter(int id)
        {
            if (Fighters.FindAll(x => x.ID == id && !x.IsDead).Count > 0)
            {
                return Fighters.FirstOrDefault(x => x.ID == id && !x.IsDead);
            }
            return null;
        }

        public FightTeam GetTeam(int id)
        {
            if (id == 0)
            {
                return RedTeam;
            }
            else
            {
                return BlueTeam;
            }
        }

        public FightTeam WinTeam
        {
            get
            {
                if (BlueTeam.Fighters.FindAll(x => !x.IsDead).Count > 0)
                {
                    return BlueTeam;
                }
                else
                {
                    return RedTeam;
                }
            }
        }

        public FightTeam LoosersTeam
        {
            get
            {
                if (BlueTeam.Fighters.FindAll(x => !x.IsDead).Count == 0)
                {
                    return BlueTeam;
                }
                else
                {
                    return RedTeam;
                }
            }
        }

        #endregion

        #region Fight Engine

        public void RefreshPreTurn()
        {
            StringBuilder pattern = new StringBuilder("GTM");
            foreach (Fighter fighter in this.Fighters)
            {
                pattern.Append("|").Append(fighter.ID.ToString()).Append(";");
                if (fighter.IsDead)
                {
                    pattern.Append("1;");
                }
                else
                {
                    pattern.Append("0;").Append(fighter.CurrentLife.ToString()).Append(";");
                    pattern.Append(fighter.CurrentAP.ToString()).Append(";").Append(fighter.CurrentMP.ToString()).Append(";");
                    pattern.Append(fighter.CellID.ToString()).Append(";;").Append(fighter.Stats.MaxLife);
                }
            }
            this.Send(pattern.ToString());
        }

        public void FinishTurnRequest(Fighter fighter)
        {
            if (fighter.ID == this.TimeLine.CurrentFighter.ID)
            {
                this.CheckEndTurnChallenge(fighter);
                fighter.ResetPoints();
                if (fighter.NextCell != -1)
                {
                    PlayerEndMove(fighter);
                }
                this.TimeLine.NextPlayer();
            }
        }

        public void PlayerWantMove(Fighter fighter, string path)
        {
            if (this.State == FightState.Fighting && this.TimeLine.CurrentFighter.ID == fighter.ID)
            {
                Engines.Pathfinding pathfinding = new Engines.Pathfinding(path, this.Map.Map,
                                                                        fighter.CellID, fighter.Dir);

                int distance = pathfinding.GetDistanceBetween(fighter.CellID, pathfinding.Destination);

                if (fighter.CurrentMP >= distance)
                {
                    string remakePath = pathfinding.GetStartPath + pathfinding.RemakePath();

                    if (fighter.ByWearFighter != null)
                    {
                        fighter.UnWear();
                    }

                    fighter.Send("GAS" + fighter.ID);
                    this.Send("GA0;1;" + fighter.ID + ";" + remakePath);

                    /* Remove MP */
                    fighter.CurrentMP -= distance;

                    fighter.NextCell = pathfinding.Destination;
                    fighter.NextDir = pathfinding.NewDirection;
                }
                else
                {
                    fighter.Send("BN");
                }
            }
            else
            {
                fighter.Send("BN");
            }
        }

        public void PlayerEndMove(Fighter fighter)
        {
            if (this.State == FightState.Fighting && this.TimeLine.CurrentFighter.ID == fighter.ID)
            {
                if (fighter.NextCell != -1 && fighter.NextDir != -1)
                {
                    int distance = this.Map.PathfindingMaker.GetDistanceBetween(fighter.CellID, fighter.NextCell);
                    this.Send("GA;129;" + fighter.ID + ";" + fighter.ID + ",-" + distance);
                    fighter.Send("GAF2|" + fighter.ID);

                    /* Set new position */
                    fighter.CellID = fighter.NextCell;
                    fighter.Dir = fighter.NextDir;

                    if (fighter.WearedFighter != null)
                    {
                        fighter.WearedFighter.CellID = fighter.CellID;
                        fighter.WearedFighter.Dir = fighter.Dir;
                    }

                    /* Reset next position result for next move */
                    fighter.NextCell = -1;
                    fighter.NextDir = -1;

                    fighter.CurrentUsedPmThisTurn += distance;
                    this.CheckZombieChallenge(fighter);

                    CheckTraps(fighter);
                }
            }
            else
            {
                fighter.Send("BN");
            }
        }

        public void CastSpell(Fighter fighter, Game.Spells.WorldSpell spell, int level, int cellid)
        {
            Engines.Spells.SpellLevel spellLevel = spell.Template.Engine.GetLevel(level);

            if (fighter.CurrentAP >= spellLevel.CostPA)
            {
                if (!fighter.CanCastSpell(spellLevel.Engine.Spell.ID))
                {
                    fighter.Client.Action.SystemMessage("Le cooldown du sort n'est pas encore lever !");
                    return;
                }

                fighter.Send("GAS" + fighter.ID);
                fighter.CurrentAP -= spellLevel.CostPA;

                bool failedHit = false;

                if (Utilities.Basic.Rand(0, spellLevel.TauxEC) == 1)
                {
                    failedHit = true;
                }

                if (!failedHit)
                {
                    this.Send("GA;300;" + fighter.ID + ";" + spell.Template.ID + ","
                        + cellid + "," + spell.Template.SpriteID + "," + level + "," + spell.Template.SpriteInfos);

                    bool criticatHit = false;
                    int tauxCC = spellLevel.TauxCC - fighter.Stats.CriticalBonus.Total;
                    if (tauxCC < 2) tauxCC = 2;

                    if (Utilities.Basic.Rand(0, tauxCC) == 1)
                    {
                        criticatHit = true;
                    }

                    if (fighter.IsHuman)
                    {
                        if (fighter.Client.Action.GodMode)
                        {
                            criticatHit = true;
                        }
                    }

                    if (criticatHit && spellLevel.Effects.FindAll(x => x.Effect != Enums.SpellsEffects.DoNothing && x.Effect != Enums.SpellsEffects.None).Count > 0)
                    {
                        this.Send("GA;301;" + fighter.ID + ";" + spell.SpellID);
                        this.ApplyEffects(fighter, spellLevel, spellLevel.Effects, cellid, criticatHit, false);
                    }
                    else
                    {
                        this.ApplyEffects(fighter, spellLevel, spellLevel.CriticalEffects, cellid, criticatHit, false);
                    }

                    //Apply new cooldown (FIXED IN THE 1.0.2.1)
                    if (spellLevel.TurnNumber > 0)
                    {
                        if (!fighter.Cooldowns.ContainsKey(spellLevel.Engine.Spell.ID))
                        {
                            fighter.Cooldowns.Add(spellLevel.Engine.Spell.ID, new FightSpellCooldown(spellLevel.Engine.Spell.ID, spellLevel.TurnNumber));
                        }
                    }
                }
                else
                {
                    this.Send("GA;302;" + fighter.ID + ";" + spell.SpellID);
                    if (spellLevel.ECCanEndTurn)
                    {
                        FinishTurnRequest(fighter);
                    }
                }

                this.Send("GA;102;" + fighter.ID + ";" + fighter.ID + ",-" + spellLevel.CostPA);
                fighter.Send("GAF0|" + fighter.ID);
            }
            else
            {
                fighter.Client.Action.SystemMessage("Le nombre de PA requis est supérieur aux votre actuellement !");
            }
        }

        public void UseWeapon(Fighter fighter, int cellID)
        {
            if (fighter.Character.Items.GetItemAtPos(1) != null)
            {
                fighter.Send("GAS" + fighter.ID);

                Database.Records.WorldItemRecord item = fighter.Character.Items.GetItemAtPos(1);
                 
                bool failedHit = false;

                if (Utilities.Basic.Rand(0, item.GetTemplate.Engine.TauxEC) == 1)
                {
                    failedHit = true;
                }

                if (!failedHit)
                {
                    if (fighter.CurrentAP >= item.Engine.CostInPa)
                    {
                        fighter.CurrentAP -= item.Engine.CostInPa;

                        bool criticatHit = false;
                        int tauxCC = item.Engine.TauxCC - fighter.Character.Stats.CriticalBonus.Total;
                        if (tauxCC < 2) tauxCC = 2;

                        if (Utilities.Basic.Rand(0, tauxCC) == 1)
                        {
                            criticatHit = true;
                        }

                        if (fighter.Client.Action.GodMode)
                        {
                            criticatHit = true;
                        }

                        this.Send("GA;303;" + fighter.ID + ";" + cellID);
                        List<Fighter> targets = GetFighterInZoneForWeapon(fighter, item.GetTemplate.Type, cellID);
                        foreach (Fighter target in targets)
                        {
                            foreach (World.Handlers.Items.Effect effect in item.Engine.Effects)
                            {
                                if (item.Engine.IsWeaponEffect(effect.ID))
                                {
                                    int baseDamage = 0;
                                    if (criticatHit)
                                    {
                                        this.Send("GA;301;" + fighter.ID + ";");
                                        baseDamage = Utilities.Basic.Rand(effect.Des.Min, effect.Des.Max);
                                    }
                                    else
                                    {
                                        baseDamage = Utilities.Basic.Rand(effect.Des.Min, effect.Des.Max);
                                    }
                                    if (GetElementForEffect(effect.ID) != 0)
                                    {
                                        int damages = FightSpellEffects.RandomDamages(baseDamage, fighter, GetElementForEffect(effect.ID));
                                        target.TakeDamages(target.ID, -damages, GetElementForEffect(effect.ID));
                                    }
                                    else if(GetStealElementForEffect(effect.ID) != 0)
                                    {
                                        int damages = FightSpellEffects.RandomDamages(baseDamage, fighter, GetStealElementForEffect(effect.ID));
                                        int stealedLife = (int)Math.Truncate((double)(damages / 2));
                                        target.TakeDamages(target.ID, -damages, GetStealElementForEffect(effect.ID));
                                        fighter.Heal(fighter.ID, stealedLife, GetStealElementForEffect(effect.ID));
                                    }
                                }
                            }
                        }
                    }
                    this.Send("GA;102;" + fighter.ID + ";" + fighter.ID + ",-" + item.Engine.CostInPa);
                }
                this.Send("GAF0|" + fighter.ID);
            }
        }

        public void ApplyEffects(Fighter fighter, Engines.Spells.SpellLevel spellLevel, List<Engines.Spells.SpellEffect> effects,
                                int cellid, bool cc, bool IsTrap, List<Fighter> onTrap = null)
        {
            int effectNum = 0;

            foreach (Engines.Spells.SpellEffect effect in effects)
            {
                try
                {
                    #region Zone

                    string viewEffect = "Pa";
                    try
                    {
                        string viewType = cc ? spellLevel.TypePO.Substring(spellLevel.Effects.Count * 2) : spellLevel.TypePO.Substring(0, spellLevel.Effects.Count * 2);
                        viewEffect = viewType.Substring(effectNum * 2, 2);
                    }
                    catch (Exception e)
                    {
                        viewEffect = "Pa";
                    }

                    List<Fighter> targets = null;

                    if (IsTrap)
                    {
                        targets = onTrap;
                    }
                    else
                    {
                        targets = GetFighterInZone(viewEffect, cellid);
                    }

                    #endregion

                    #region Targets

                    if (effect.Targets != null)
                    {
                        //Check targets
                        if (!effect.Targets.Ennemies)
                        {
                            targets.ToArray().ToList().FindAll(x => !fighter.Team.IsFriendly(x)).ForEach(x => targets.Remove(x));
                        }
                        if (!effect.Targets.Friends)
                        {
                            targets.ToArray().ToList().FindAll(x => fighter.Team.IsFriendly(x)).ForEach(x => targets.Remove(x));
                        }
                        if (!effect.Targets.Caster)
                        {
                            if (targets.Contains(fighter))
                                targets.Remove(fighter);
                        }
                        if (effect.Targets.CasterPlus)
                        {
                            if (!targets.Contains(fighter))
                                targets.Add(fighter);
                        }

                        //Manual fix
                        //Dissolution
                        if (spellLevel.Engine.Spell.ID == 439)
                        {
                            if (targets.Contains(fighter))
                                targets.Remove(fighter);
                        }
                    }

                    //Reverse spell
                    foreach (var target in targets.ToArray())
                    {
                        if (target.HasReverseSpellBuff() != null && !target.Team.IsFriendly(fighter))
                        {
                            if (target.HasReverseSpellBuff().Level >= spellLevel.Level)
                            {
                                targets.Remove(target);
                                targets.Add(fighter);
                                this.Send("GA;106;" + target.ID + ";" + target.ID + ",1");
                            }
                        }
                    }

                    #endregion

                    FightSpellEffects.ApplyEffect(this, effect, spellLevel, fighter, cellid, targets);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error : " + e.ToString());
                }
                effectNum++;
            }
        }

        public void EndFightResult()
        {
            System.Threading.Thread.Sleep(2500);

            /* //////// Build end packet //////// */

            try
            {
                string packet = "GE";
                long elapsedTimeFight = Environment.TickCount - this.StartTime;

                packet += elapsedTimeFight + "|" + this.ID + "|" + (this.FightType == Enums.FightTypeEnum.Agression ? "1" : "0");

                this.GenerateDrops();

                foreach (Fighter player in Fighters.FindAll(x => !x.IsInvoc))
                {
                    bool winner = WinTeam.IsFriendly(player);

                    packet += "|" + (this.WinTeam.IsFriendly(player) ? "2" : "0");
                    packet += ";" + player.ID;
                    packet += ";" + player.Nickname;
                    packet += ";" + player.Level;
                    packet += ";" + (player.IsDead ? "1" : "0");

                    switch (this.FightType)
                    {
                        #region Challenge

                        case Enums.FightTypeEnum.Challenge:
                            int earnedExperience = 0;
                            int earnedKamas = 0;

                            if (Utilities.ConfigurationManager.GetBoolValue("EnableExpInPvPFight") && winner)
                            {
                                earnedExperience = LoosersTeam.Fighters.FindAll(x => !x.IsInvoc).Count * Utilities.Basic.Formulas_ExpPvp(player.Team.GetTeamLevel(), LoosersTeam.GetTeamLevel());
                                player.Client.Action.AddExp(earnedExperience);
                            }

                            if (Utilities.ConfigurationManager.GetIntValue("EarnedPvPKamasPerEnnemis") > 0 && winner)
                            {
                                earnedKamas = LoosersTeam.Fighters.FindAll(x => !x.IsInvoc).Count * Utilities.ConfigurationManager.GetIntValue("EarnedPvPKamasPerEnnemis");
                                player.Character.Kamas += earnedKamas;
                            }

                            packet += ";" + player.Character.ExpFloor.Character;
                            packet += ";" + player.Character.Experience;
                            if (Helper.ExpFloorHelper.GetNextCharactersLevelFloor(player.Character.Level) != null)
                            {
                                packet += ";" + Helper.ExpFloorHelper.GetNextCharactersLevelFloor(player.Character.Level).Character;
                            }
                            else
                            {
                                packet += ";-1";
                            }
                            packet += ";" + earnedExperience;
                            packet += ";";
                            packet += ";";
                            packet += ";";
                            packet += ";" + earnedKamas;
                            break;

                        #endregion

                        #region PVM

                        case Enums.FightTypeEnum.PvM:
                            long earnedPvmExperience = 0;
                            int earnedPvmKamas = 0;

                            if (WinTeam.IsFriendly(player))
                            {
                                foreach (Fighter mob in LoosersTeam.Fighters)
                                {
                                    if (!mob.IsHuman)
                                    {
                                        earnedPvmKamas += Utilities.Basic.Rand(mob.Monster.GetTemplate.IntervallKamas[0], mob.Monster.GetTemplate.IntervallKamas[1]);
                                    }
                                }
                                if (player.IsHuman)
                                {
                                    player.Character.Kamas += earnedPvmKamas;
                                }
                                if (player.IsHuman)
                                {
                                    earnedPvmExperience = ExpPvmFormulas(player);
                                    player.Client.Action.AddExp(earnedPvmExperience);
                                }
                            }

                            if (player.IsHuman)
                            {
                                packet += ";" + player.Character.ExpFloor.Character;
                                packet += ";" + player.Character.Experience;
                                if (Helper.ExpFloorHelper.GetNextCharactersLevelFloor(player.Character.Level) != null)
                                {
                                    packet += ";" + Helper.ExpFloorHelper.GetNextCharactersLevelFloor(player.Character.Level).Character;
                                }
                                else
                                {
                                    packet += ";-1";
                                }
                                packet += ";" + earnedPvmExperience;
                            }
                            else
                            {
                                packet += ";;;;";
                            }
                            packet += ";";
                            packet += ";";
                            packet += ";" + player.Drops.Parse;
                            packet += ";" + earnedPvmKamas;
                            break;

                        #endregion

                        #region Aggresion

                        case Enums.FightTypeEnum.Agression:
                            int earnedHonor = (Utilities.ConfigurationManager.GetIntValue("RateHonor") * Utilities.ConfigurationManager.GetIntValue("BaseCoefHonor")) 
                                * LoosersTeam.Fighters.FindAll(x => !x.IsInvoc).Count;

                            if (WinTeam.IsFriendly(player))
                            {
                                player.Character.Faction.AddExp(earnedHonor);
                            }
                            else
                            {
                                player.Character.Faction.RemoveExp(earnedHonor);
                            }

                            packet += ";" + player.Character.Faction.Floor.Pvp;
                            packet += ";" + player.Character.Faction.Honor;
                            packet += ";" + (Helper.ExpFloorHelper.GetNextCharactersLevelFloor(player.Character.Faction.Power) != null ?
                                Helper.ExpFloorHelper.GetNextCharactersLevelFloor(player.Character.Faction.Power).Pvp.ToString() : "-1");
                            if (WinTeam.IsFriendly(player))
                            {
                                packet += ";" + earnedHonor;
                            }
                            else
                            {
                                packet += ";" + (-(earnedHonor));
                            }
                            packet += ";" + player.Character.Faction.Power;
                            break;

                        #endregion
                    }
                }

                this.Send(packet);

            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant send end fight result : " + e.ToString());
            }

            /* //////// Ending build end packet //////// */

        }

        #endregion

        #region Challenges

        public void InitChallenges()
        {
            for (int i = 0; i <= 1; i++)
            {
                var randomChalNumb = Utilities.Basic.Rand(1, 5);
                Challenges.FightChallenge newChallenge = null;
                switch (randomChalNumb)
                {
                    case 1:
                        newChallenge = new Challenges.FirstKillChallenge(this, RedTeam, BlueTeam.Fighters.FirstOrDefault());
                        break;

                    case 2:
                        newChallenge = new Challenges.ZombieChallenge(this, RedTeam);
                        break;

                    case 3:
                        newChallenge = new Challenges.HealChallenge(this, RedTeam);
                        break;

                    case 4:
                        newChallenge = new Challenges.NomadeChallenge(this, RedTeam);
                        break;
                }
                this.Challenges.Add(newChallenge);
                newChallenge.ShowChallenges();
            }
        }

        public void CheckMonsterKilledChallenge(Fighter killer, Fighter monster)
        {
            try
            {
                //if (RedTeam.IsFriendly(killer)) BUG WHY
                //{
                    foreach (var chal in this.Challenges)
                    {
                        chal.OnMosterDie(killer, monster);
                    }
                //}
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't check challenge : " + e.ToString());
            }
        }

        public void CheckZombieChallenge(Fighter moved)
        {
            try
            {
                if (RedTeam.IsFriendly(moved))
                {
                    foreach (var chal in this.Challenges)
                    {
                        chal.OnMove(moved, moved.CurrentUsedPmThisTurn);
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't check challenge : " + e.ToString());
            }
        }

        public void CheckHealChallenge(Fighter healer, Fighter healed)
        {
            try
            {
                //if (healer.Team.IsFriendly(healed))
                //{
                    foreach (var chal in this.Challenges)
                    {
                        chal.OnHeal(healer, healed);
                    }
                //}
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't check challenge : " + e.ToString());
            }
        }

        public void CheckEndTurnChallenge(Fighter fighter)
        {
            try
            {
                if (RedTeam.IsFriendly(fighter))
                {
                    foreach (var chal in this.Challenges)
                    {
                        chal.OnEndTurn(fighter);
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't check challenge : " + e.ToString());
            }
        }

        #endregion

        #region Traps Engine

        public void AddTrap(FightTrap trap)
        {
            this.Traps.Add(trap);
            trap.Owner.Team.Send("GA;999;" + trap.Owner.ID + ";GDZ+" + trap.CellID + ";" + trap.Lenght + ";7");
            trap.OwnerSend("GA;999;" + trap.Owner.ID + ";GDC" + trap.CellID + ";Haaaaaaaaz3005;");
        }

        public void AddGlyph(FightTrap trap)
        {
            this.Glyphs.Add(trap);
            Send("GA;999;" + trap.Owner.ID + ";GDZ+" + trap.CellID + ";" + trap.Lenght + ";" + trap.TrapColor);
        }

        #endregion

        #region Methods

        public void Send(string packet)
        {
            this.Fighters.ForEach(x => x.Send(packet));
            this.Spectators.ForEach(x => x.Send(packet));
        }

        public void QuitBattle(Fighter fighter, bool loop = false, bool abandon = false)
        {
            this.Send("GM|-" + fighter.ID);
            if (fighter.IsHuman)
            {
                if (FightType == Enums.FightTypeEnum.PvM)
                {
                    fighter.Character.CurrentLife = 1;
                    World.Network.World.GoToMap(fighter.Client, fighter.Client.Character.SaveMap, fighter.Client.Character.SaveCell);
                }
                else if (abandon && FightType == Enums.FightTypeEnum.Agression)
                {
                    fighter.Client.Character.FactionHonor -= 250;
                    fighter.Client.Action.SystemMessage("Vous avez perdu <b>250</b> points d'honneur suite a votre abandon");
                }
                else if (FightType == Enums.FightTypeEnum.Kolizeum)
                {
                    fighter.Client.Action.KolizeumMessage("Votre combat est terminer, vos points de vie on ete remis au maximum !");
                    fighter.Character.CurrentLife = fighter.Character.Stats.MaxLife;
                    World.Network.World.GoToMap(fighter.Client, fighter.Client.Action.KolizeumLastMapID, fighter.Client.Action.KolizeumLastCellID);
                }
            }
            fighter.Team.Fighters.Remove(fighter);
            switch (this.State)
            {
                case FightState.PlacementsPhase:
                    if (!IsAvailableBattle() && !loop)
                    {
                        foreach (Fighter otherFighter in Fighters)
                        {
                            /* Loop is a true, for prevent stack overflow exception */
                            this.QuitBattle(otherFighter, true);
                        }
                        this.EndFightEngine();
                        this.Map.RemoveFightOnMap(this);
                    }
                    break;

                case FightState.Fighting:
                    if (!IsAvailableBattle() && !loop)
                    {
                        foreach (Fighter otherFighter in Fighters)
                        {
                            /* Loop is a true, for prevent stack overflow exception */
                            this.QuitBattle(otherFighter, true);
                        }
                        this.EndFightResult();
                        this.RedTeam.Fighters.Clear();
                        this.BlueTeam.Fighters.Clear();
                        this.EndFightEngine();
                        this.Map.RemoveFightOnMap(this);
                        this.TimeLine.RemixTimeLine();
                    }
                    break;
            }
            fighter.Buffs.ForEach(x => x.BuffRemoved());
            fighter.Buffs.Clear();
            if (fighter.IsHuman)
            {
                fighter.Character.Fighter = null;
            }
            fighter.Send("GV");

        }

        public void QuitSpectator(FightSpectator spectator)
        {
            this.Spectators.Remove(spectator);
            spectator.Send("GV");
        }

        public void FighterDisconnection(Fighter fighter)
        {
            switch (this.State)
            {
                case FightState.PlacementsPhase:
                    this.QuitBattle(fighter);
                    break;

                case FightState.Fighting:
                    this.QuitBattle(fighter);
                    break;
            }
        }

        public void EndFightEngine()
        {
            this.CheckTimer.Enabled = false;
            this.CheckTimer.Close();
            this.CheckTimer.Stop();
            this.TimeLine.EndTimeline();
            this.Spectators.ForEach(x => QuitSpectator(x));
            if (Helper.DungeonRoomHelper.IsDungeonRoom(this.Map.Map.ID) && this.FightType == Enums.FightTypeEnum.PvM)
            {
                Database.Records.DungeonRoomRecord room = Helper.DungeonRoomHelper.GetDungeonRoom(this.Map.Map.ID);
                foreach (Fighter winner in WinTeam.Fighters)
                {
                    if (winner.IsHuman)
                    {
                        World.Network.World.GoToMap(winner.Client, room.ToMap, room.ToCell);
                    }
                }
                foreach (Fighter looser in this.LoosersTeam.Fighters)
                {
                    if (looser.IsHuman)
                    {
                        //World.Network.World.GoToMap(looser.Client, looser.Client.Character.SaveMap, looser.Client.Character.SaveCell);
                    }
                }
            }
            else if (FightType == Enums.FightTypeEnum.PvM)
            {
                foreach (Fighter looser in this.LoosersTeam.Fighters)
                {
                    if (looser.IsHuman)
                    {
                        looser.Character.CurrentLife = 1;
                        World.Network.World.GoToMap(looser.Client, looser.Client.Character.SaveMap, looser.Client.Character.SaveCell);
                    }
                }
                //foreach (Fighter winner in WinTeam.Fighters)
                //{
                //    if (winner.IsHuman)
                //    {
                //        Interop.PythonScripting.ScriptManager.CallEventWinBattleVersusMonster(winner.Client, this.Map.Map.ID);
                //    }
                //}
            }
            else if (FightType == Enums.FightTypeEnum.Kolizeum)
            {
                foreach (var fighter in Fighters)
                {
                    try
                    {
                        if (fighter.IsHuman)
                        {
                            fighter.Client.Action.Teleport(fighter.Client.Action.KolizeumLastMapID, fighter.Client.Action.KolizeumLastCellID);
                        }
                    }
                    catch (Exception e)
                    {
                        //TODO
                    }
                }
            }
        }

        public bool IsAvailableBattle()
        {
            if (this.BlueTeam.Fighters.Count < 1 || this.RedTeam.Fighters.Count < 1)
            {
                return false;
            }
            return true;
        }

        public void CheckEnd()
        {
            try
            {
                bool endingOfFight = false;

                foreach (var fighter in Fighters)
                {
                    if (fighter.CurrentLife <= 0)
                    {
                        fighter.IsDead = true;
                    }
                }

                if (this.RedTeam.Fighters.FindAll(x => !x.IsDead).Count <= 0)
                {
                    endingOfFight = true;
                }
                if (this.BlueTeam.Fighters.FindAll(x => !x.IsDead).Count <= 0)
                {
                    endingOfFight = true;
                }

                //Utilities.ConsoleStyle.Debug("Try battle finish");

                if (endingOfFight && this.Alive)
                {
                    this.Alive = false;
                    this.Map.RemoveFightOnMap(this);
                    this.Challenges.FindAll(x => x.IsAlive).ForEach(x => x.ChallengeSuccess());
                    this.EndFightResult();
                    this.EndFightEngine();
                    System.Threading.Thread.Sleep(1000);
                    foreach (Fighter otherFighter in Fighters)
                    {
                        if (FightType == Enums.FightTypeEnum.Challenge)
                        {
                            otherFighter.CurrentLife = otherFighter.StartLife;
                        }
                        if (otherFighter.IsHuman)
                        {
                            otherFighter.Character.Fighter = null;
                        }
                        otherFighter.Send("GV");
                        otherFighter.Buffs.ForEach(x => x.BuffRemoved());
                        otherFighter.Buffs.Clear();
                        otherFighter.Stats.RefreshStats();
                    }
                    this.RedTeam.Fighters.Clear();
                    this.BlueTeam.Fighters.Clear();
                    Utilities.ConsoleStyle.Debug("Battle finished");
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant check end : " + e.ToString());
            }
        }

        public List<Fighter> GetFighterInZone(string zone, int baseCell)
        {
            List<Fighter> fighters = new List<Fighter>();
            if (zone == "")
                zone = "Pa";
            switch (zone[0])
            {
                default:
                case 'P':
                    Fighter singleFighterCell = GetFighterOnCell(baseCell);
                    if (singleFighterCell != null)
                        fighters.Add(singleFighterCell);
                    break;

                case 'C':
                    List<int> cellsCircle = Engines.Pathfinding.GetCircleZone(baseCell, Engines.Pathfinding.GetDirNum(zone[1].ToString()), Map.Map);
                    foreach (int cell in cellsCircle)
                    {
                        Fighter circleFighterCell = GetFighterOnCell(cell);
                        if (circleFighterCell != null)
                            fighters.Add(circleFighterCell);
                    }
                    break;

                case 'X':
                    List<int> cellsCross = Engines.Pathfinding.GetCrossZone(baseCell, Engines.Pathfinding.GetDirNum(zone[1].ToString()), Map.Map);
                    foreach (int cell in cellsCross)
                    {
                        Fighter crossFighterCell = GetFighterOnCell(cell);
                        if (crossFighterCell != null)
                            fighters.Add(crossFighterCell);
                    }
                    break;
            }
            return fighters;
        }

        public List<Fighter> GetFighterInZoneForWeapon(Fighter caster, int weaponType, int baseCell)
        {
            List<Fighter> fighters = new List<Fighter>();
            switch (weaponType)
            {
                case 7://Marteau
                    var cellsMarteau = Engines.Pathfinding.GetCrossZone(baseCell, 1, this.Map.Map);
                    foreach (var cell in cellsMarteau)
                    {
                        if (GetFighterOnCell(cell) != null)
                        {
                            if (GetFighterOnCell(cell).ID != caster.ID)
                            {
                                fighters.Add(GetFighterOnCell(cell));
                            }
                        }
                    }
                    break;

                default:
                case 2://Bow
                case 5://Dagger
                case 6://Sword
                    Fighter singleFighterCell = GetFighterOnCell(baseCell);
                    if (singleFighterCell != null)
                        fighters.Add(singleFighterCell);
                    break;
            }
            return fighters;
        }

        public int GetElementForEffect(int effect)
        {
            int element = 0;
            switch (effect)
            {
                case 96://Eau
                    return 3;

                case 100://Neutre
                case 97://Terre
                    return 1;

                case 98://Air
                    return 4;

                case 99://Feu
                    return 2;
            }
            return element;
        }

        public int GetStealElementForEffect(int effect)
        {
            int element = 0;
            switch ((Enums.SpellsEffects)effect)
            {
                case Enums.SpellsEffects.VolEau:
                    return 3;

                case Enums.SpellsEffects.VolAir:
                    return 4;

                case Enums.SpellsEffects.VolNeutre:
                case Enums.SpellsEffects.VolTerre:
                    return 1;

                case Enums.SpellsEffects.VolFeu:
                    return 2;
            }
            return element;
        }

        public void CheckTraps(Fighter fighter)
        {
            foreach (FightTrap trap in this.Traps.ToArray())
            {
                if (trap.WalkOnTrap(fighter.CellID))
                {
                    this.Send("GA1;306;" + fighter.ID + ";" + trap.SpellLevel.Engine.Spell.ID + "," + trap.CellID + ",407,1,1," + trap.Owner.ID);
                    this.Traps.Remove(trap);

                    trap.UseEffect(fighter.CellID);

                    trap.Owner.Team.Send("GA;999;" + trap.Owner.ID + ";GDZ-" + trap.CellID+";" + trap.Lenght + ";7");
                    trap.OwnerSend("GA;999;" + trap.Owner.ID + ";GDC" + trap.CellID);
                }
            }
        }

        public bool IsTrapOnCell(int cellid)
        {
            return this.Traps.FindAll(x => x.WalkOnTrap(cellid)).Count > 0;
        }

        #endregion

        #region Enums

        public enum FightState
        {
            PlacementsPhase = 0,
            Fighting = 1,
        }

        #endregion

        #region Drops Engine

        public void GenerateDrops()
        {
            var possibleDrops = new Dictionary<int, int>();
            var monsterLoots = new List<Database.Records.DropRecord>();

            /* Generate looted droppred items */
            foreach (var monster in this.LoosersTeam.Fighters.FindAll(x => !x.IsHuman && !x.IsInvoc))
            {
                foreach (var drop in monster.Monster.GetTemplate.MonsterDrops)
                {
                    for (int i = 0; i <= drop.Quantity - 1; i++)
                    {
                        if (CanDrop(drop))
                        {
                            if (possibleDrops.ContainsKey(drop.ItemID))
                            {
                                possibleDrops[drop.ItemID]++;
                            }
                            else
                            {
                                possibleDrops.Add(drop.ItemID, 1);
                            }
                        }
                    }
                }
            }

            var winHumanTeam = this.WinTeam.Fighters.FindAll(x => x.IsHuman && !x.IsInvoc);

            /* Assign drop to fighter */
            foreach (var loot in possibleDrops)
            {
                for (int i = 0; i <= loot.Value - 1; i++)
                {
                    if (winHumanTeam.Count > 0)
                    {
                        var randomFighter = winHumanTeam[Utilities.Basic.Rand(0, winHumanTeam.Count)];
                        randomFighter.Drops.AddDrop(loot.Key);
                    }
                }
            }

            winHumanTeam.ForEach(x => x.Drops.GenerateInInventory());
        }

        public bool CanDrop(Database.Records.DropRecord drop)
        {
            return Utilities.Basic.Rand(0, 100) <= drop.Rate;
        }

        #endregion

        #region Formulas

        public long ExpPvmFormulas(Fighter player)
        {
            long baseExp = 0;
            this.LoosersTeam.Fighters.FindAll(x => !x.IsHuman && !x.IsInvoc).ForEach(x => baseExp += x.Monster.GetTemplate.Exp);
            baseExp = baseExp * Utilities.ConfigurationManager.GetIntValue("RatePvM");
            if (player.IsHuman)
            {
                if (player.Client.Account.Vip == 1)
                {
                    baseExp = baseExp * 2;
                }
            }
            return baseExp;
        }

        #endregion

    }
}