using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public class CraftCache
    {
        public static List<Records.CraftRecord> Cache = new List<Records.CraftRecord>();

        public static void Init()
        {
            Cache = Records.CraftRecord.FindAll().ToList();
            Cache.ForEach(x => x.Initialize());
        }
    }
}
