using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : PaddockHelper
*/

namespace Crystal.WorldServer.World.Helper
{
    public static class PaddockHelper
    {
        public static Database.Records.PaddockRecord FindPaddock(int mapid)
        {
            if (Database.Cache.PaddockCache.Cache.FindAll(x => x.MapID == mapid).Count > 0)
                return Database.Cache.PaddockCache.Cache.FirstOrDefault(x => x.MapID == mapid);
            return null;
        }

        public static List<Database.Records.WorldMountRecord> GetMountForOwner(int owner)
        {
            return Database.Cache.WorldMountCache.Cache.FindAll(x => x.Owner == owner);
        }
    }
}
