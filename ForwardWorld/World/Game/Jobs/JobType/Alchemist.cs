using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Jobs.JobType
{
    public class Alchemist : Job
    {
        public override int JobID { get { return (int)Enums.JobsIDEnums.Alchemist; } }
        public override int[] Tools { get { return new int[1] { 1473 }; } }
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
                    new JobSkill(this, 23, 1, 50, 1, true, 5000),//Préparer une potion
                    new JobSkill(this, 68, 1, 5, 1, false, 5000, 10),//Lin
                };
            }
        }

        public Alchemist(long experiences, int level)
            : base(experiences, level) {
        }
    }
}
