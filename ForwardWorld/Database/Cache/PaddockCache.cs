using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : PaddockCache
*/

namespace Crystal.WorldServer.Database.Cache
{
    public static class PaddockCache
    {
        public static List<Records.PaddockRecord> Cache = new List<Records.PaddockRecord>();

        public static void Init()
        {
            Cache = Records.PaddockRecord.FindAll().ToList();
            foreach (var paddock in Cache)
            {
                Database.Records.MapRecords map = World.Helper.MapHelper.FindMap(paddock.MapID);
                if (map != null)
                {
                    map.Engine.Paddocks.Add(paddock);
                }
            }
        }
    }
}
