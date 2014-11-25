using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal.WorldServer.World.Game.Fights.Challenges
{
    public class FirstKillChallenge : FightChallenge
    {
        public FirstKillChallenge(Fight fight, FightTeam team, Fighter toKill)
            : base(fight, team, Enums.ChallengesTypeID.FirstDie)
        {
            this.Target = toKill;
        }

        public override int EarnedDrop
        {
            get
            {
                return 40;
            }
        }

        public override int EarnedExp
        {
            get
            {
                return 40;
            }
        }

        public override void OnMosterDie(Fighter fighter, Fighter monster)
        {
            if (this.IsAlive)
            {
                if (monster.ID == this.Target.ID)
                {
                    this.ChallengeSuccess();
                }
                else
                {
                    this.ChallengeFailed(monster);
                }
            }
        }
    }
}
