using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Helper
{
    public static class MonsterHelper
    {
        public static Database.Records.MonstersTemplateRecord GetMonsterTemplate(int id)
        {
            if (Database.Cache.MonstersTemplateCache.Cache.FindAll(x => x.ID == id).Count > 0)
                return Database.Cache.MonstersTemplateCache.Cache.FirstOrDefault(x => x.ID == id);
            return null;
        }
    }
}
