using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public class JobDataCache
    {
        public static List<Records.JobDataRecord> Cache = new List<Records.JobDataRecord>();

        public static void Init()
        {
            Cache = Records.JobDataRecord.FindAll().ToList();
            Cache.ForEach(x => x.Initialize());
        }
    }
}
