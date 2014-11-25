using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class AddArmorBuff : Engines.Spells.SpellBuff
    {
        public int Value { get; set; }

        public AddArmorBuff(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            
        }

        public override void ApplyBuff()
        {
            
        }
    }
}
