using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Jobs.JobType
{
    public class Paysan : Job
    {
        public override int JobID { get { return (int)Enums.JobsIDEnums.Farmer; } }
        public override int[] Tools { get { return new int[1] { 577 }; } }
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
                    new JobSkill(this, 122, 1, 5, 1, true, 5000),//Egrener
                    new JobSkill(this, 47, 1, 5, 1, true, 5000),//Egrener
                    new JobSkill(this, 45, 1, 5, 1, false, 5000, 10),//Ble
                    new JobSkill(this, 53, 1, 5, 10, false, 5000, 15),//Orge
                };
            }
        }

        public Paysan(long experiences, int level)
            : base(experiences, level) {
        }
    }
}
