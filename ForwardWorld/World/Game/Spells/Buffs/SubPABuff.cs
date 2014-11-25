using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class SubPABuff: Engines.Spells.SpellBuff
    {
        public int Value = 0;

        public SubPABuff(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void ApplyBuff()
        {
            BuffedFighter.CurrentAP -= Value;
            BuffedFighter.Stats.ActionPoints.Bonus -= Value;
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            BuffedFighter.CurrentAP += Value;
            BuffedFighter.Stats.ActionPoints.Bonus += Value;
        }
    }
}
