using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class MonstersTemplateCache
    {
        public static List<Records.MonstersTemplateRecord> Cache = new List<Records.MonstersTemplateRecord>();

        public static void Init()
        {
            Cache = Records.MonstersTemplateRecord.FindAll().ToList();
        }
    }
}
