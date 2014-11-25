using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class AddPMBuff : Engines.Spells.SpellBuff
    {
        public int Value = 0;

        public AddPMBuff(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void ApplyBuff()
        {
            BuffedFighter.CurrentMP += Value;
            BuffedFighter.Stats.MovementPoints.Bonus += Value;
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            BuffedFighter.CurrentMP -= Value;
            BuffedFighter.Stats.MovementPoints.Bonus -= Value;
        }
    }
}
