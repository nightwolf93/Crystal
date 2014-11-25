using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class NpcCache
    {
        public static List<Records.NpcRecord> Cache = new List<Records.NpcRecord>();

        public static void Init()
        {
            Cache = Records.NpcRecord.FindAll().ToList();
        }
    }
}
