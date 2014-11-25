using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crystal.WorldServer.World.Game.Fights;

namespace Crystal.WorldServer.Engines.Spells
{
    public class SpellBuff
    {
        public int Duration = 0;
        public bool CanDebuff = true;

        public Fighter BuffedFighter { get; set; }

        public World.Game.Fights.FighterState StateType = FighterState.None;

        public SpellBuff(int duration, bool canDebuff, Fighter fighter)
        {
            this.Duration = duration;
            this.CanDebuff = canDebuff;
            this.BuffedFighter = fighter;
        }

        public virtual void ApplyBuff()
        {
            throw new NotImplementedException();
        }

        public virtual void RemovedOneTurDuration()
        {
            this.Duration--;
        }

        public virtual void BuffRemoved()
        {
            throw new NotImplementedException();
        }

        public virtual void FighterHit(int damages) { }
    }
}
