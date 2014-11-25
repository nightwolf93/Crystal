using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.IO
{
    public static class InteractiveObjectHelper
    {
        public static Database.Records.IODataRecord GetIOData(int id)
        {
            if (Database.Cache.IODataCache.Cache.FindAll(x => x.ID == id).Count > 0)
            {
                return Database.Cache.IODataCache.Cache.FirstOrDefault(x => x.ID == id);
            }
            else
            {
                return null;
            }
        }
    }
}
