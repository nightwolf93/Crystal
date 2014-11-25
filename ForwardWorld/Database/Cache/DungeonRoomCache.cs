using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class DungeonRoomCache
    {
        public static List<Records.DungeonRoomRecord> Cache = new List<Records.DungeonRoomRecord>();

        public static void Init()
        {
            Cache = Records.DungeonRoomRecord.FindAll().ToList();
        }
    }
}
