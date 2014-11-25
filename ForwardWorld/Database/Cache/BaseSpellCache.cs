using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class BaseSpellCache
    {
        public static List<Database.Records.BaseSpellRecord> Cache = new List<Records.BaseSpellRecord>();

        public static void Init()
        {
            Cache = Database.Records.BaseSpellRecord.FindAll().ToList();
        }
    }
}
