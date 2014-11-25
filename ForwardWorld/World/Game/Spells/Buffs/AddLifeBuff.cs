using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class AddLifeBuff : Engines.Spells.SpellBuff
    {
        public int Value = 0;

        public AddLifeBuff(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void ApplyBuff()
        {
            BuffedFighter.CurrentLife += Value;
            BuffedFighter.Stats.Life.Bonus += Value;
            BuffedFighter.Stats.RefreshStats();
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            BuffedFighter.CurrentLife -= Value;
            BuffedFighter.Stats.Life.Bonus -= Value;
        }
    }
}
