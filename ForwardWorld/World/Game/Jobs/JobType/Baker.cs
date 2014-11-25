using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Jobs.JobType
{
    public class Baker : Job
    {
        public override int JobID { get { return (int)Enums.JobsIDEnums.Baker; } }
        public override int[] Tools { get { return new int[1] { 492 }; } }
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
                    new JobSkill(this, 109, 1, 50, 1, true, 5000),//Préparer une potion
                    new JobSkill(this, 27, 1, 5, 1, true, 5000),//Lin
                };
            }
        }

        public Baker(long experiences, int level)
            : base(experiences, level) {
        }
    }
}
