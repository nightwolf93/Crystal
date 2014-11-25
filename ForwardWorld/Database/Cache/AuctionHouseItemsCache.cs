using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public class AuctionHouseItemsCache
    {
        public static List<Records.AuctionHouseItemRecord> Cache = new List<Records.AuctionHouseItemRecord>();

        public static void Init()
        {
            Cache = Records.AuctionHouseItemRecord.FindAll().ToList();
        }
    }
}
