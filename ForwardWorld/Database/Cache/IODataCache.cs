using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class IODataCache
    {
        public static List<Records.IODataRecord> Cache = new List<Records.IODataRecord>();

        public static void Init()
        {
            Cache = Records.IODataRecord.FindAll().ToList();
        }
    }
}
