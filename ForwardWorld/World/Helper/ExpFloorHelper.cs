using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Helper
{
    public static class ExpFloorHelper
    {
        public static Database.Records.ExpFloorRecord GetCharactersFloor(long exp)
        {
            if (Database.Cache.ExpFloorCache.Cache.FindAll(x => x.Character <= exp).Count > 0)
            {
                return Database.Cache.ExpFloorCache.Cache.LastOrDefault(x => x.Character <= exp);
            }
            else
            {
                return null;
            }         
        }

        public static Database.Records.ExpFloorRecord GetCharactersLevelFloor(int level)
        {
            if (Database.Cache.ExpFloorCache.Cache.FindAll(x => x.ID == level).Count > 0)
            {
                return Database.Cache.ExpFloorCache.Cache.FirstOrDefault(x => x.ID == level);
            }
            else
            {
                return null;
            }
        }

        public static Database.Records.ExpFloorRecord GetNextCharactersLevelFloor(int level)
        {
            if (Database.Cache.ExpFloorCache.Cache.FindAll(x => x.ID == level + 1).Count > 0)
            {
                return Database.Cache.ExpFloorCache.Cache.FirstOrDefault(x => x.ID == level + 1);
            }
            else
            {
                return null;
            }
        }

        public static Database.Records.ExpFloorRecord GetCharactersPvPFloor(long exp)
        {
            if (exp < 0) exp = 0;
            List<Database.Records.ExpFloorRecord> reversedList = Database.Cache.ExpFloorCache.Cache.ToArray().ToList();
            reversedList.Reverse();
            if (reversedList.FindAll(x => x.Pvp <= exp && x.Pvp != -1).Count > 0)
            {
                return reversedList.FirstOrDefault(x => x.Pvp <= exp && x.Pvp != -1);
            }
            else
            {
                return reversedList.FirstOrDefault(x => x.ID == 10);
            }
        }

        public static Database.Records.ExpFloorRecord GetJobLevelFloor(int level)
        {
            if (level > 100) return null;
            if (Database.Cache.ExpFloorCache.Cache.FindAll(x => x.ID == level).Count > 0)
            {
                return Database.Cache.ExpFloorCache.Cache.FirstOrDefault(x => x.ID == level);
            }
            else
            {
                return null;
            }
        }

        public static Database.Records.ExpFloorRecord GetNextJobLevelFloor(int level)
        {
            if (level + 1 > 100) return null;
            if (Database.Cache.ExpFloorCache.Cache.FindAll(x => x.ID == level + 1).Count > 0)
            {
                return Database.Cache.ExpFloorCache.Cache.FirstOrDefault(x => x.ID == level + 1);
            }
            else
            {
                return null;
            }
        }

        public static Database.Records.ExpFloorRecord GetJobFloorExp(long exp)
        {
            if (Database.Cache.ExpFloorCache.Cache.FindAll(x => x.Job <= exp && x.Job != -1).Count > 0)
            {
                return Database.Cache.ExpFloorCache.Cache.LastOrDefault(x => x.Job <= exp && x.Job != -1);
            }
            else
            {
                return null;
            }
        }
    }
}
