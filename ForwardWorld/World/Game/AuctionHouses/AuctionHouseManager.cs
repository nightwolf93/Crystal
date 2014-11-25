using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.AuctionHouses
{
    public class AuctionHouseManager
    {
        public static Database.Records.AuctionHouseRecord GetAuctionHouse(int mapID, int npcID)
        {
            lock (Database.Cache.AuctionHouseCache.Cache)
            {
                if (Database.Cache.AuctionHouseCache.Cache.FindAll(x => x.MapID == mapID && x.NpcID == npcID).Count > 0)
                {
                    return Database.Cache.AuctionHouseCache.Cache.FirstOrDefault(x => x.MapID == mapID && x.NpcID == npcID);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
