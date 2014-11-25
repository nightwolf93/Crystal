using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class InvisibleBuff : Engines.Spells.SpellBuff
    {
        public int Value = 0;

        public InvisibleBuff(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void ApplyBuff()
        {
            BuffedFighter.States.Add(Fights.FighterState.Invisible);
            BuffedFighter.Team.Fight.Send("GA;150;" + BuffedFighter.ID + ";" + BuffedFighter.ID + "," + this.Duration);
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            BuffedFighter.UnInvisible();
        }
    }
}
