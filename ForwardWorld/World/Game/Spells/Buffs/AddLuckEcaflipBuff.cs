using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class AddLuckEcaflipBuff : Engines.Spells.SpellBuff
    {
        public AddLuckEcaflipBuff(int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            
        }

        public override void ApplyBuff()
        {
            
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved() { }
    }
}
