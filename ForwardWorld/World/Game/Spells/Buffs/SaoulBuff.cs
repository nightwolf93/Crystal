using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Spells.Buffs
{
    public class SaoulBuff : Engines.Spells.SpellBuff
    {
        public int Value = 0;

        public SaoulBuff(int value, int duration, Fights.Fighter fighter)
            : base(duration, true, fighter) 
        {
            this.Value = value;
        }

        public override void ApplyBuff()
        {
            BuffedFighter.States.Add(Fights.FighterState.Saoul);
            BuffedFighter.Team.Fight.Send("GA;950;" + BuffedFighter.ID + ";" + BuffedFighter.ID + "," + Value + ",1");
        }

        public override void RemovedOneTurDuration()
        {
            base.RemovedOneTurDuration();
        }

        public override void BuffRemoved()
        {
            BuffedFighter.States.Remove(Fights.FighterState.Saoul);
            BuffedFighter.Team.Fight.Send("GA;950;" + BuffedFighter.ID + ";" + BuffedFighter.ID + "," + Value + ",0"); 
        }
    }
}
