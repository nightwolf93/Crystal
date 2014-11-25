using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class TriggerCache
    {
        public static List<Records.TriggerRecord> Cache = new List<Records.TriggerRecord>();

        public static void Init()
        {
            foreach (var map in MapCache.Cache)
            {
                map.Triggers.Clear();
            }
            Cache = Records.TriggerRecord.FindAll().ToList();
            Cache.ForEach(x => World.Helper.MapHelper.AssignTrigger(x));
        }
    }
}
