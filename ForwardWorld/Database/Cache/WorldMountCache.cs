using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : WorldMountCache
*/

namespace Crystal.WorldServer.Database.Cache
{
    public static class WorldMountCache
    {
        public static List<Records.WorldMountRecord> Cache = new List<Records.WorldMountRecord>();

        public static void Init()
        {
            Cache = Records.WorldMountRecord.FindAll().ToList();
            foreach (var mount in Cache)
            {
                mount.LoadStats();
            }
        }
    }
}
