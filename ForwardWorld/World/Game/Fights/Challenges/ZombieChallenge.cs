using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal.WorldServer.World.Game.Fights.Challenges
{
    public class ZombieChallenge : FightChallenge
    {
        public ZombieChallenge(Fight fight, FightTeam team)
            : base(fight, team, Enums.ChallengesTypeID.Zombie) { }

        public override int EarnedDrop
        {
            get
            {
                return 25;
            }
        }

        public override int EarnedExp
        {
            get
            {
                return 25;
            }
        }

        public override void OnMove(Fighter fighter, int value)
        {
            if (value > 1)
            {
                this.ChallengeFailed(fighter);
            }
        }
    }
}
