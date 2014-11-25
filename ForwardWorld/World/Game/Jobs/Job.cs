using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Jobs
{
    public abstract class Job
    {
        //JS = JobInfos
        //JN = JobLevelUp

        #region Fields

        public virtual int JobID { get; set; }
        public long Experience { get; set; }
        public int Level { get; set; }
        public bool ShowedInPublicBook { get; set; }
        public virtual List<JobSkill> Skills { get; set; }
        public virtual int[] Tools { get; set; }
        public virtual int[] Objects { get; set; }

        #endregion

        #region CTor

        public Job(long experience, int level)
        {
            this.Experience = experience;
            this.Level = level;
        }

        #endregion

        #region Book

        /// <summary>
        /// Show character in the craft book
        /// </summary>
        public virtual void ShowInPublicBook()
        {
            if (!ShowedInPublicBook)
            {
                this.ShowedInPublicBook = true;
            }
        }

        /// <summary>
        /// Hide character in the craft book
        /// </summary>
        public virtual void HideInPublicBook()
        {
            if (ShowedInPublicBook)
            {
                this.ShowedInPublicBook = false;
            }
        }

        #endregion

        #region Leveling

        public Database.Records.ExpFloorRecord LevelFloor
        {
            get
            {
                return Helper.ExpFloorHelper.GetJobLevelFloor(this.Level);
            }
        }

        public Database.Records.ExpFloorRecord NextLevelFloor
        {
            get
            {
                return Helper.ExpFloorHelper.GetNextJobLevelFloor(this.Level);
            }
        }

        public void AddExp(int exp, World.Network.WorldClient client = null)
        {
            try
            {
                //Only add exp if is needed
                if (this.Level < 100 && this.Level > 0)
                {
                    this.Experience += exp;
                    var possibleNextLevel = Helper.ExpFloorHelper.GetJobFloorExp(this.Experience);
                    if (possibleNextLevel != null)
                    {
                        if (possibleNextLevel.ID != this.LevelFloor.ID)
                        {
                            this.Level = possibleNextLevel.ID;
                            if (client != null)
                            {
                                client.Send("JN" + this.JobID + "|" + this.Level);
                            }
                        }
                    }
                    this.SendJob(client);
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't add exp to the job : " + e.ToString());
            }
        }

        #endregion

        #region Getting

        public JobSkill GetSkill(int skill)
        {
            if (this.Skills.FindAll(x => x.ID == skill && x.Level <= this.Level).Count > 0)
            {
                return this.Skills.FirstOrDefault(x => x.ID == skill && x.Level <= this.Level);
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return this.JobID + "," + this.Level + "," + this.Experience;
        }

        #endregion

        #region Displaying

        public void SendJobInfos(World.Network.WorldClient client)
        {
            var packet = "JX|" + this.JobID + ";" + this.Level + ";";
            if (this.LevelFloor != null)
            {
                packet += this.LevelFloor.Job;
            }
            else
            {
                packet += this.Experience;
            }
            packet += ";" + this.Experience + ";";
            if (this.NextLevelFloor != null)
            {
                packet += this.NextLevelFloor.Job;
            }
            else
            {
                packet += this.Experience;
            }
            packet += ";";
            client.Send(packet);
        }

        public void SendJob(Network.WorldClient client)
        {
            this.SendJobSkill(client);
            this.SendJobInfos(client);
            this.SendJobTool(client);
        }

        public void SendJobSkill(World.Network.WorldClient client)
        {
            var packet = new StringBuilder("JS");
            packet.Append("|").Append(this.JobID).Append(";");
            var skills = "";
            foreach (var s in this.Skills.FindAll(x => x.Level <= this.Level))
            {
                if (skills != "") skills += ",";
                skills += s.ToJS();
            }
            packet.Append(skills);
            client.Send(packet.ToString());
        }

        public void SendJobTool(World.Network.WorldClient client)
        {
            if (client.Character.Items.GetItemAtPos(1) != null)
            {
                var item = client.Character.Items.GetItemAtPos(1);
                if (this.Tools.Contains(item.Template))
                {
                    var packet = new StringBuilder("OT");
                    packet.Append(this.JobID);
                    client.Send(packet.ToString());
                }
            }
        }

        #endregion
    }
}
