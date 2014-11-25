using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Crystal.WorldServer.World.Game.Jobs
{
    public static class JobManager
    {
        public static Dictionary<Enums.JobsIDEnums, Type> Jobs = new Dictionary<Enums.JobsIDEnums, Type>();

        public static void LoadJobs()
        {
            Jobs.Add(Enums.JobsIDEnums.Farmer, typeof(JobType.Paysan));
            Jobs.Add(Enums.JobsIDEnums.Alchemist, typeof(JobType.Alchemist));
            Jobs.Add(Enums.JobsIDEnums.Lumberjack, typeof(JobType.Lumberjack));
            Jobs.Add(Enums.JobsIDEnums.Tailor, typeof(JobType.Tailor));
            Jobs.Add(Enums.JobsIDEnums.Baker, typeof(JobType.Baker));
        }

        public static Type GetJobType(Enums.JobsIDEnums jobID)
        {
            if (Jobs.ContainsKey(jobID))
            {
                return Jobs[jobID];
            }
            return null;
        }

        public static Job CreateNewJob(Enums.JobsIDEnums jobID)
        {
            Type jobType = GetJobType(jobID);
            if (jobType != null)
            {
                return (Job)Activator.CreateInstance(jobType, new object[2] { 0, 1 });
            }
            else
            {
                return null;
            }
        }

        public static void LearnJob(Network.WorldClient client, Enums.JobsIDEnums jobType)
        {
            if (!client.Character.HaveJob(jobType))
            {
                if (client.Character.Jobs.Count < 3)
                {
                    var job = Game.Jobs.JobManager.CreateNewJob(jobType);
                    client.Character.Jobs.Add(job);

                    client.Action.RefreshCharacterJob();
                }
                else
                {
                    client.Action.SystemMessage("Vous possedez trop de metier pour en apprendre un nouveau !");
                }
            }
            else
            {
                client.Action.SystemMessage("Vous connaissez deja ce métier !");
            }
        }
    }
}
