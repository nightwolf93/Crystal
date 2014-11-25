using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public class ItemBagCache
    {
        public static List<Records.ItemBagRecord> Cache = new List<Records.ItemBagRecord>();

        public static void Init()
        {
            Cache = Records.ItemBagRecord.FindAll().ToList();
        }
    }
}
