using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class ShopItemCache
    {
        public static List<Records.ShopItemRecord> Cache = new List<Records.ShopItemRecord>();

        public static void Init()
        {
            Cache = Records.ShopItemRecord.FindAll().ToList();
        }
    }
}
