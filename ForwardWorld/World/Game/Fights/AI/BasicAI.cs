using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Engines.Pathfinder;

namespace Crystal.WorldServer.World.Game.Fights.AI
{
    public class BasicAI : MonsterAI
    {
        public BasicAI(Fighter monster)
            : base(monster) { }

        public override void StartIA()
        {
            try
            {
                this.Monster.ResetPoints();
                this.LaunchBestSpell();
                this.NextMove = this.BestMoves();
                this.Move();
                this.LaunchBestSpell();
                this.EndTurn();
            }
            catch (Exception e)
            {
                this.EndTurn();
            }
        }

        public List<int> BestMoves(bool far = false)
        {
            if (this.Monster.LifePercentage <= 10)
            {
                return MoveFar();
            }
            List<int> moves = new List<int>();
            Fighter nearestFighter = GetNearestFighter();
            int mp = Monster.CurrentMP;
            int baseCell = Monster.CellID;
            var pathEngine = new PathfindingV2(this.MonsterFight.Map);
            var path = pathEngine.FindShortestPath(baseCell, nearestFighter.CellID, this.GetDynObs(nearestFighter.CellID));
            foreach (var cell in path)
            {
                if (cell.ID != nearestFighter.CellID)
                {
                    moves.Add(cell.ID);
                    mp--;
                    if (mp == 0) break;
                }
                else
                {
                    break;
                }
            }
            return moves;
        }

        public void LaunchBestSpell()
        {
            BestSpell();
        }
    }
}
