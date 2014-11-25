using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class AddReverseDamage : Engines.Spells.SpellBuff
    {
        public int Value = 0;
        public int CalculedReversedDamages = 0;

        public AddReverseDamage(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void ApplyBuff()
        {
            //renvoi de dommage de base*(sagesse + 100)/100
            this.CalculedReversedDamages = (int)Math.Truncate((double)(this.Value * (base.BuffedFighter.Stats.Wisdom.Total + 100) / 100));
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved() { }
    }
}
