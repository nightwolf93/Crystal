using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : MountTemplateCache
*/

namespace Crystal.WorldServer.Database.Cache
{
    public static class MountTemplateCache
    {
        public static List<Records.MountTemplateRecord> Cache = new List<Records.MountTemplateRecord>();

        public static void Init()
        {
            Cache = Records.MountTemplateRecord.FindAll().ToList();
            Cache.ForEach(x => x.Engine = new Engines.MountStatsEngine(x.Effects));
        }
    }
}
