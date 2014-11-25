using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Fights
{
    public class FightRestrictions
    {
        public FightTeam SecuredTeam { get; set; }

        public bool FullBlocked = false;
        public bool OnlyParty = false;
        public bool HelpRequested = false;

        public FightRestrictions(FightTeam team)
        {
            this.SecuredTeam = team;
        }

        public bool CanJoin(Fighter fighter)
        {
            if (this.FullBlocked) return false;
            if (this.SecuredTeam.Leader.Character.Party != null && this.OnlyParty)
            {
                if (!this.SecuredTeam.Leader.Character.Party.Members.Contains(fighter.Client)) return false;
            }
            if (this.SecuredTeam.Fight.FightType == Enums.FightTypeEnum.Agression)
            {
                if (this.SecuredTeam.Leader.Character.FactionID != fighter.Character.FactionID)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
