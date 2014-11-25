using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crystal.WorldServer.Utilities;

namespace Crystal.WorldServer.Engines.Spells
{
    public class SpellTarget
    {
        public bool Ennemies = false;
        public bool Friends = false;
        public bool Caster = false;
        public bool CasterPlus = false;
        public bool Invocations = false;

        public SpellTarget(int value)
        {
            Ennemies = Basic.GetFlag(value, 0);
            Friends = Basic.GetFlag(value, 1);
            Caster = Basic.GetFlag(value, 2);
            CasterPlus = Basic.GetFlag(value, 3);
            Invocations = Basic.GetFlag(value, 4);
        }
    }
}
