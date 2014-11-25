using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : AddWaterBuff
*/

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class AddWaterBuff: Engines.Spells.SpellBuff
    {
        public int Value = 0;

        public AddWaterBuff(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void ApplyBuff()
        {
            BuffedFighter.Stats.Water.Bonus += Value;
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            BuffedFighter.Stats.Water.Bonus -= Value;
        }
    }
}
