using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal.WorldServer.World.Game.Fights.Challenges
{
    public class NomadeChallenge : FightChallenge
    {
        public override int EarnedDrop
        {
            get
            {
                return 55;
            }
        }

        public override int EarnedExp
        {
            get
            {
                return 55;
            }
        }

        public NomadeChallenge(Fight fight, FightTeam team)
            : base(fight, team, Enums.ChallengesTypeID.Nomade) { }

        public override void OnEndTurn(Fighter fighter)
        {
            if (fighter.CurrentMP > 0)
            {
                this.ChallengeFailed(fighter);
            }
        }
    }
}
