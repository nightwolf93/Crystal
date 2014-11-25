using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Database.Cache
{
    public static class SpellCache
    {
        public static List<Records.SpellRecord> Cache = new List<Records.SpellRecord>();

        public static void Init()
        {
            Cache = Records.SpellRecord.FindAll().ToList();
            foreach (Records.SpellRecord spell in Cache)
            {
                spell.Engine = new Engines.SpellEngine(spell);
                spell.Engine.LoadLevels();
            }
        }
    }
}
