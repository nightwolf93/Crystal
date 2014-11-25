using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public class AuctionHouseCache
    {
        public static List<Records.AuctionHouseRecord> Cache = new List<Records.AuctionHouseRecord>();

        public static void Init()
        {
            Cache = Records.AuctionHouseRecord.FindAll().ToList();
        }
    }
}
