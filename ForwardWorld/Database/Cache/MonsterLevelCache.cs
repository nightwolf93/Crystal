using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class MonsterLevelCache
    {
        public static List<Records.MonsterLevelRecord> Cache = new List<Records.MonsterLevelRecord>();

        public static void Init()
        {
            Cache = Records.MonsterLevelRecord.FindAll().ToList();
            Cache.ForEach(x => x.InitMonster());
        }
    }
}
