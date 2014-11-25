using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Fights.AI
{
    public class StaticAI : MonsterAI
    {
        public StaticAI(Fighter monster)
            : base(monster) { }

        public override void StartIA()
        {
            this.EndTurn();
        }
    }
}
