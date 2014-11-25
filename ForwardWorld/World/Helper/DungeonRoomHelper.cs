using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Helper
{
    public static class DungeonRoomHelper
    {
        public static bool IsDungeonRoom(int mapid)
        {
            return Database.Cache.DungeonRoomCache.Cache.FindAll(x => x.MapID == mapid).Count > 0;
        }

        public static Database.Records.DungeonRoomRecord GetDungeonRoom(int mapID)
        {
            if (IsDungeonRoom(mapID))
            {
                return Database.Cache.DungeonRoomCache.Cache.FirstOrDefault(x => x.MapID == mapID);
            }
            else
            {
                return null;
            }
        }
    }
}
