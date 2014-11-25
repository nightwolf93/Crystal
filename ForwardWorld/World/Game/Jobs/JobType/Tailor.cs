using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Jobs.JobType
{
    public class Tailor : Job
    {
        public override int JobID { get { return (int)Enums.JobsIDEnums.Tailor; } }
        public override int[] Tools { get { return new int[1] { 951 }; } }
        public override int[] Objects
        {
            get
            {
                return base.Objects;
            }
        }
        public override List<JobSkill> Skills
        {
            get
            {
                return new List<JobSkill>()
                {
                    new JobSkill(this, 64, 1, 5, 1, true, 5000),//Sac
                    new JobSkill(this, 123, 1, 5, 1, true, 5000),//Cape
                    new JobSkill(this, 63, 1, 5, 1, true, 5000),//Chapeau
                };
            }
        }

        public Tailor(long experiences, int level)
            : base(experiences, level) {
        }
    }
}
