using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : AddFireBuff
*/

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class AddFireBuff: Engines.Spells.SpellBuff
    {
        public int Value = 0;

        public AddFireBuff(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void ApplyBuff()
        {
            BuffedFighter.Stats.Fire.Bonus += Value;
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            BuffedFighter.Stats.Fire.Bonus -= Value;
        }
    }
}
