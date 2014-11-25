using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Fights
{
    public class FightTrap
    {
        public Fighter Owner { get; set; }
        public Engines.Spells.SpellLevel SpellLevel { get; set; }
        public int CellID { get; set; }
        public int Lenght { get; set; }
        public Enums.FightTrapType TrapType = Enums.FightTrapType.TRAP;

        public Engines.Spells.SpellLevel UsedEffect { get; set; }

        public int TrapColor = -1;
        public int TurnDuration = 0;

        public List<int> CurrentZone = new List<int>();

        public FightTrap(Fighter owner, int spellID, int level, Engines.Spells.SpellLevel spellLevel, int cellID, int lenght, Enums.FightTrapType type)
        {
            this.Owner = owner;
            this.SpellLevel = spellLevel;
            this.CellID = cellID;
            this.Lenght = lenght;
            this.TrapType = type;
            this.ProcessCellsZone();
            this.UsedEffect = Helper.SpellHelper.GetSpell(spellID).Engine.GetLevel(level);
        }

        public FightTrap(Fighter owner, int spellID, int level, Engines.Spells.SpellLevel spellLevel, int cellID, int lenght, Enums.FightTrapType type, int trapColor, int turn)
        {
            this.Owner = owner;
            this.SpellLevel = spellLevel;
            this.CellID = cellID;
            this.Lenght = lenght;
            this.TrapType = type;
            this.TrapColor = trapColor;
            this.TurnDuration = turn;
            this.ProcessCellsZone();
            this.UsedEffect = Helper.SpellHelper.GetSpell(spellID).Engine.GetLevel(level);
            this.Owner.OwnGlyph.Add(this);
        }

        public void ProcessCellsZone()
        {
            this.CurrentZone = Engines.Pathfinding.GetCircleZone(this.CellID, this.Lenght, this.Owner.Team.Fight.Map.Map);
        }

        public void OwnerSend(string packet)
        {
            if (Owner.Team.Fighters.Contains(Owner))
            {
                this.Owner.Send(packet);
            }
        }

        public bool WalkOnTrap(int cellid)
        {
            return this.CurrentZone.Contains(cellid);
        }

        public void RemoveOneTurnDuration()
        {
            TurnDuration--;
            if (this.TurnDuration <= 0)
            {
                this.RemoveGlyph();
            }
        }

        public void RemoveGlyph()
        {
            this.Owner.Team.Fight.Send("GA;999;" + this.Owner.ID + ";GDZ-" + this.CellID + ";" + this.Lenght + ";" + this.TrapColor);
            this.Owner.Team.Fight.Glyphs.Remove(this);
            this.Owner.OwnGlyph.Remove(this);
        }

        public List<Fighter> GetFighterOnTrap()
        {
            return this.Owner.Team.Fight.Fighters.FindAll(x => !x.IsDead && WalkOnTrap(x.CellID));
        }

        public void UseEffect(int cell, Fighter onlyOne = null)
        {
            if (onlyOne != null)
            {
                List<Fighter> target = new List<Fighter>();
                target.Add(onlyOne);
                this.Owner.Team.Fight.ApplyEffects(Owner, UsedEffect, UsedEffect.Effects, cell, false, true, target);
            }
            else
            {
                this.Owner.Team.Fight.ApplyEffects(Owner, UsedEffect, UsedEffect.Effects, cell, false, true, GetFighterOnTrap());
            }           
        }
    }
}
