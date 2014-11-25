using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class ElitesCache
    {
        public static List<Records.ElitesRecord> Cache = new List<Records.ElitesRecord>();

        public static void Init()
        {
            Cache = Records.ElitesRecord.FindAll().ToList();
        }
    }
}
