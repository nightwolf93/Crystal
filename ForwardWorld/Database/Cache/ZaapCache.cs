using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class ZaapCache
    {
        public static List<Records.ZaapRecord> Cache = new List<Records.ZaapRecord>();

        public static void Init()
        {
            Cache = Records.ZaapRecord.FindAll().ToList();
        }
    }
}
