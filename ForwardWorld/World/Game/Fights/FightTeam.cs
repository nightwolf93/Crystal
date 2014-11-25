using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Fights
{
    public class FightTeam
    {
        public int ID;
        public Fighter Leader { get; set; }
        public Fight Fight { get; set; }
        public List<Fighter> Fighters = new List<Fighter>();
        public List<int> PlacementsPlaces = new List<int>();
        public int BladeCell { get; set; }
        public bool VirtualTeam = false;

        public FightRestrictions Restrictions { get; set; }

        public FightTeam(int id, Fighter leader, Fight fight, bool virtualTeam)
        {
            this.ID = id;
            this.Leader = leader;
            this.Fight = fight;
            this.BladeCell = this.Leader.MapCell;
            this.VirtualTeam = virtualTeam;
            this.Restrictions = new FightRestrictions(this);
            if (!this.VirtualTeam)
            {
                this.AddToTeam(leader);
            }
        }

        public void AddToTeam(Fighter fighter)
        {
            fighter.Team = this;
            this.Fighters.Add(fighter);
        }

        public bool HaveFightersOnThisPlace(int cell)
        {
            return this.Fighters.FindAll(x => x.CellID == cell).Count > 0;
        }

        public string DisplayPatternBladeTeam
        {
            get
            {
                StringBuilder pattern = new StringBuilder(this.ID.ToString());

                if (VirtualTeam)
                {
                    this.Fighters.ForEach(x => pattern.Append("|+")
                        .Append(x.ID.ToString())
                        .Append(";")
                        .Append(x.Monster.GetTemplate.Name)
                        .Append(";")
                        .Append(x.Monster.Level));
                }
                else
                {
                    this.Fighters.ForEach(x => pattern.Append("|+")
                        .Append(x.Character.ID.ToString())
                        .Append(";")
                        .Append(x.Character.Nickname)
                        .Append(";")
                        .Append(x.Character.Level.ToString()));
                }

                return pattern.ToString();
            }
        }

        public void AddFighter(Fighter fighter)
        {
            this.Fighters.Add(fighter);
            fighter.Team = this;
            if(Fight.State == Fights.Fight.FightState.PlacementsPhase)
                this.Fight.Map.Send("Gt" + this.ID + "|+" + fighter.ID + ";" + fighter.Nickname + ";" + fighter.Level);
        }

        public bool IsFriendly(Fighter fighter)
        {
            return fighter.Team.ID == this.ID;
        }

        public int GetTeamLevel()
        {
            int level = 0;
            Fighters.ForEach(x => level += x.Level);
            return level;
        }

        public void Send(string packet)
        {
            this.Fighters.ForEach(x => x.Send(packet));
        }

        public bool AnyoneAlive()
        {
            return this.Fighters.FindAll(x => !x.IsDead).Count > 1;
        }
    }
}
