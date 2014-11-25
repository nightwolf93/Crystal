using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class BannedAccountCache
    {
        public static List<Records.BannedAccountRecord> Cache = new List<Records.BannedAccountRecord>();

        public static void Init()
        {
            Cache = Records.BannedAccountRecord.FindAll().ToList();
        }
    }
}
