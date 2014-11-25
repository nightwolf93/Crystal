using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Jobs.JobType
{
    public class Lumberjack : Job
    {
        public override int JobID { get { return (int)Enums.JobsIDEnums.Lumberjack; } }
        public override int[] Tools { get { return new int[1] { 454 }; } }
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
                    new JobSkill(this, 101, 1, 5, 1, true, 5000),//Scier
                    new JobSkill(this, 6, 1, 5, 1, false, 5000, 12),//Frene
                    new JobSkill(this, 39, 1, 5, 10, false, 5000, 15),//Chataigner
                    new JobSkill(this, 40, 1, 5, 20, false, 5000, 20),//Noyer
                    new JobSkill(this, 10, 1, 5, 30, false, 5000, 25),//Chene
                    new JobSkill(this, 141, 1, 5, 35, false, 5000, 27),//Oliviolet
                    new JobSkill(this, 139, 1, 5, 35, false, 5000, 27),//Bombu
                    new JobSkill(this, 37, 1, 5, 40, false, 5000, 30),//Bombu
                    new JobSkill(this, 154, 1, 5, 50, false, 5000, 35),//Bambou
                    new JobSkill(this, 33, 1, 5, 50, false, 5000, 35),//If
                    new JobSkill(this, 41, 1, 5, 60, false, 5000, 40),//Merisier
                    new JobSkill(this, 34, 1, 5, 70, false, 5000, 45),//Ebene
                    new JobSkill(this, 174, 1, 5, 75, false, 5000, 47),//Kalyptus
                    new JobSkill(this, 155, 1, 5, 80, false, 5000, 50),//Boie de bambou sombre
                    new JobSkill(this, 38, 1, 5, 80, false, 5000, 50),//Charme
                    new JobSkill(this, 35, 1, 5, 90, false, 5000, 55),//Orme
                    new JobSkill(this, 158, 1, 5, 100, false, 5000, 0),//Bambou sacree
                };
            }
        }

        public Lumberjack(long experiences, int level)
            : base(experiences, level) {
        }
    }
}
