using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Engines.Spells;
using Crystal.WorldServer.World.Game.Fights;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    /// <summary>
    /// Reverse spell like feca spell
    /// </summary>
    public class AddReverseSpellBuff : SpellBuff
    {
        public int Level { get; set; }

        public AddReverseSpellBuff(int duration, int level, Fighter fighter)
            : base(duration, true, fighter)
        {
            this.Level = level;
        }

        public override void FighterHit(int damages)
        {
            base.FighterHit(damages);
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
