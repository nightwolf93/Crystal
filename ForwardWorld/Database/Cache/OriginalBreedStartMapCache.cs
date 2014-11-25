using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class OriginalBreedStartMapCache
    {
        public static List<Records.OriginalBreedStartMapRecord> Cache = new List<Records.OriginalBreedStartMapRecord>();

        public static void Init()
        {
            Cache = Records.OriginalBreedStartMapRecord.FindAll().ToList();
        }
    }
}
