using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal.WorldServer.World.Game.Fights.Challenges
{
    public class HealChallenge : FightChallenge
    {
        public HealChallenge(Fight fight, FightTeam team)
            : base(fight, team, Enums.ChallengesTypeID.Heal) { }

        public override int EarnedDrop
        {
            get
            {
                return 15;
            }
        }

        public override int EarnedExp
        {
            get
            {
                return 15;
            }
        }

        public override void OnHeal(Fighter healer, Fighter healed)
        {
            if (healer.Team != null && healed != null)
            {
                if (healer.Team.IsFriendly(healed))
                {
                    this.ChallengeFailed(healer);
                }
            }
        }
    }
}
