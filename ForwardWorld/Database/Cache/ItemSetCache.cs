using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class ItemSetCache
    {
        public static List<Records.ItemSetRecord> Cache = new List<Records.ItemSetRecord>();

        public static void Init()
        {
            Cache = Records.ItemSetRecord.FindAll().ToList();
            Cache.ForEach(x => x.Initialize());
        }
    }
}
