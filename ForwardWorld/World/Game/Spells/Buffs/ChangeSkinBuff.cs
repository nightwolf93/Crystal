using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class ChangeSkinBuff : Engines.Spells.SpellBuff
    {
        public int Value = 0;

        public ChangeSkinBuff(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void ApplyBuff() { }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            BuffedFighter.Team.Fight.Send("GA;149;" + BuffedFighter.ID + ";" + BuffedFighter.ID + 
                "," + BuffedFighter.Look + "," + BuffedFighter.Look + ",-1");
        }
    }
}
