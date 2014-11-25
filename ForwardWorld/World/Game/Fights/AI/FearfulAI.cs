using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : FearfulAI
*/

namespace Crystal.WorldServer.World.Game.Fights.AI
{
    public class FearfulAI : MonsterAI
    {
        public FearfulAI(Fighter monster)
            : base(monster) { }


        public override void StartIA()
        {
            try
            {
                this.NextMove = MoveUntilCanHit();
                this.Move();
                this.BestSpell();
                this.NextMove = MoveFar();
                this.Move();
                this.EndTurn();
            }
            catch (Exception e)
            {
                this.EndTurn();
            }
        }

        public List<int> MoveUntilCanHit()
        {
            List<int> moves = new List<int>();
            List<int> closedList = new List<int>();
            Fighter nearestFighter = GetNearestFighter();
            int mp = Monster.CurrentMP;
            int baseCell = Monster.CellID;
            int timeout = 0;
            while (mp != 0)
            {
                closedList.Add(baseCell);
                if (MonsterFight.Map.PathfindingMaker.GetDistanceBetween(baseCell, nearestFighter.CellID) == 1)
                    break;
                int nextCell = GetNearestCellForGoingToFighter(nearestFighter, baseCell, closedList);
                if (nextCell != -1)
                {
                    moves.Add(nextCell);
                    baseCell = nextCell;
                    if (CanHit(baseCell))
                    {
                        break;
                    }
                }
                mp--;
                timeout++;
                if (timeout > 100)
                    break;
            }
            return moves;
        }
    }
}
