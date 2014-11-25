using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Fights
{
    public class FightSpellCooldown
    {
        public int SpellID { get; set; }
        public int Time { get; set; }

        public FightSpellCooldown(int spellid, int time)
        {
            this.SpellID = spellid;
            this.Time = time;
        }

        public void Remove()
        {
            this.Time--;
        }
    }
}
