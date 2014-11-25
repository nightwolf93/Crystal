using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class BreedCache
    {
        public static List<Records.BreedRecord> Cache = new List<Records.BreedRecord>();

        public static void Init()
        {
            Cache = Records.BreedRecord.FindAll().ToList();
            Cache.ForEach(x => x.Engine = new Engines.BreedFloorEngine(x));
        }
    }
}
