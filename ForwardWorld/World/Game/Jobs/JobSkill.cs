using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Jobs
{
    public class JobSkill
    {
        public Job BaseJob { get; set; }
        public int ID { get; set; }
        public bool IsCraftSkill { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int Time { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }

        public JobSkill(Job job, int id, int min, int max, int level, bool craftSkill = true, int time = 0, int exp = 0)
        {
            this.BaseJob = job;
            this.ID = id;
            this.IsCraftSkill = craftSkill;
            this.Min = min;
            this.Max = max;
            this.Time = time;
            this.Level = level;
            this.Exp = exp;
        }

        public int GetDesByLevel()
        {
            return ((this.BaseJob.Level - this.Level + 1) / 5) + 2;
        }

        public double GetJobTime()
        {
            float t = (float)(12 + ((float)this.Level / 10)) - ((float)this.BaseJob.Level / 10);
            if (t < 1)
            {
                t = 1;
            }
            return (float)t * 1000f;
        }

        public int GetJobCraftMax()
        {
            return (int)Math.Floor((double)(this.BaseJob.Level / 10)) + 2;
        }

        public double GetCraftChance(int recipesCount)
        {
            if (this.GetJobCraftMax() - 2 >= recipesCount) return 99.9f;
            if (this.BaseJob.Level < 10) return 50;
            return 54 + (int)((this.BaseJob.Level / 10) - 1) * 5;
        }

        public string ToJS()
        {
            if (this.IsCraftSkill)
            {
                return this.ID + "~" + this.GetJobCraftMax() + "~0~0~" + this.GetCraftChance(this.GetJobCraftMax());//The max value is getChance();
            }
            else
            {
                return this.ID + "~" + this.Min + "~" + this.GetDesByLevel() + "~0~" + this.GetJobTime();
            }
        }

        public bool DoSkill(Network.WorldClient client, IO.InteractiveObject io)
        {
            if (!this.IsCraftSkill)
            {
                if (io.State == IO.InteractiveObjectState.FULL)
                {
                    client.Action.RefreshDirection(3);//TODO: From player direction

                    var packet = "GA" + this.ID + ";501;" + client.Character.ID + ";" + io.CellID + "," + this.GetJobTime();
                    client.Character.Map.Engine.Send(packet);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (client.Action.CurrentJobCraftSkill == null)
                {
                    client.Action.CurrentJobCraftSkill = new JobCraftSkill(this, client);
                    var packet = "ECK3|" + this.GetJobCraftMax() + ";" + this.ID;
                    client.Send(packet);
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public void SkillFinished(Network.WorldClient client, IO.InteractiveObject io)
        {
            try
            {
                if (JobHelper.GetItemBySkill(this.ID) != -1)
                {
                    var quantity = Utilities.Basic.Rand(this.Min, this.GetDesByLevel());
                    Database.Records.WorldItemRecord item = Helper.ItemHelper.GenerateItem(client, JobHelper.GetItemBySkill(this.ID));
                    client.Character.AddItem(item, quantity);

                    client.Send("IQ" + client.Character.ID + "|" + quantity);
                    client.Action.RefreshPods();
                    this.BaseJob.AddExp(this.Exp, client);
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't finish job skill : " + e.ToString());
            }

            io.SetEmpty();
            io.StartRespawnTimer();
        }
    }
}
