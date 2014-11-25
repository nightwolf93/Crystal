using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Manager
{
    public static class WorldManager
    {
        public static Network.WorldServer Server;
        public static List<string> MutedAccount = new List<string>();

        public static void InitServer()
        {
            Server = new Network.WorldServer(Utilities.ConfigurationManager.GetStringValue("WorldHost"),
                                                    Utilities.ConfigurationManager.GetIntValue("WorldPort"));
        }

        public static void SyncMonsterLevelWithTemplate()
        {
            foreach (Database.Records.MonsterLevelRecord levels in Database.Cache.MonsterLevelCache.Cache)
            {
                Database.Records.MonstersTemplateRecord template = Helper.MonsterHelper.GetMonsterTemplate(levels.TemplateID);
                if (template != null)
                {
                    if (template.Levels == null)
                    {
                        template.Levels = new List<Database.Records.MonsterLevelRecord>();
                    }
                    template.Levels.Add(levels);
                }
            }
        }

        public static void SyncMapWithMonsterAvailable()
        {
            foreach (Database.Records.MapRecords map in Database.Cache.MapCache.Cache)
            {
                map.Engine.Spawner.AddStringMonstersData(map.Monsters);
                map.Engine.Spawner.SetRestriction(map.MaximumGroup, map.MaximumMonster);
            }
        }

        public static void SendMessage(string message)
        {
            Helper.WorldHelper.GetClientsArray.ToList().ForEach(x => x.Action.SystemMessage(message));
        }

        public static void SendMessage(string message, string color)
        {
            Helper.WorldHelper.GetClientsArray.ToList().ForEach(x => x.Action.BasicMessage(message, color));
        }

        public static bool IsBanned(Database.Records.AccountRecord account)
        {
            return Database.Cache.BannedAccountCache.Cache.FindAll(x => x.Account.ToLower() == account.Username.ToLower()).Count > 0;
        }
    }
}
