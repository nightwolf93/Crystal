using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Engines.Pathfinder;

namespace Crystal.WorldServer.World.Game.Fights.AI
{
    public abstract class MonsterAI
    {
        public Fight MonsterFight { get { return Monster.Team.Fight; } }
        public Fighter Monster { get; set; }

        public List<int> NextMove = new List<int>();

        public bool FirstTurn = true;

        public MonsterAI(Fighter monster)
        {
            this.Monster = monster;
        }

        public virtual void StartIA()
        {
            throw new NotImplementedException();
        }

        public Fighter GetNearestFighter()
        {
            Fighter nearest = null;
            int dist = 1000;
            int timeout = 0;
            foreach (Fighter fighter in MonsterFight.Fighters)
            {
                if (!fighter.IsDead && !fighter.Team.IsFriendly(Monster))
                {
                    int fDist = MonsterFight.Map.PathfindingMaker.GetDistanceBetween(this.Monster.CellID, fighter.CellID);
                    if (fDist < dist)
                    {
                        dist = fDist;
                        nearest = fighter;
                    }
                }
                timeout += 1;
                if (timeout > 100)
                    break;
            }
            return nearest;
        }

        public Fighter GetNearestFriendlyFighter()
        {
            Fighter nearest = null;
            int dist = 1000;
            int timeout = 0;
            foreach (Fighter fighter in MonsterFight.Fighters)
            {
                if (!fighter.IsDead && fighter.Team.IsFriendly(Monster) && fighter.ID != this.Monster.ID)
                {
                    int fDist = MonsterFight.Map.PathfindingMaker.GetDistanceBetween(this.Monster.CellID, fighter.CellID);
                    if (fDist < dist)
                    {
                        dist = fDist;
                        nearest = fighter;
                    }
                }
                timeout += 1;
                if (timeout > 100)
                    break;
            }
            return nearest;
        }

        public int GetNearestCellForGoingToFighter(Fighter target, int baseCell, List<int> closedList)
        {
            int cell = -1;
            List<int> adjaCells = Engines.Pathfinding.GetJoinCell(baseCell, MonsterFight.Map.Map);
            int dist = 1000;
            int timeout = 0;
            foreach (int aCell in adjaCells)
            {
                if (MonsterFight.Map.IsAvailableCell(aCell) && MonsterFight.GetFighterOnCell(aCell) == null && !closedList.Contains(aCell))
                {
                    int fDist = MonsterFight.Map.PathfindingMaker.GetDistanceBetween(aCell, target.CellID);
                    if (fDist < dist)
                    {
                        cell = aCell;
                        dist = fDist;
                    }
                }
                timeout += 1;
                if (timeout > 100)
                    break;
            }
            if (cell == -1)
                return baseCell;
            return cell;
        }

        public int GetFarestCellForGoingToFighter(Fighter target, int baseCell, List<int> closedList)
        {
            int cell = -1;
            List<int> adjaCells = Engines.Pathfinding.GetJoinCell(baseCell, MonsterFight.Map.Map);
            int dist = 0;
            int timeout = 0;
            foreach (int aCell in adjaCells)
            {
                if (MonsterFight.Map.IsAvailableCell(aCell) && MonsterFight.GetFighterOnCell(aCell) == null && !closedList.Contains(aCell))
                {
                    int fDist = MonsterFight.Map.PathfindingMaker.GetDistanceBetween(aCell, target.CellID);
                    if (fDist > dist)
                    {
                        cell = aCell;
                        dist = fDist;
                    }
                }
                timeout += 1;
                if (timeout > 100)
                    break;
            }
            return cell;
        }

        public void EndTurn()
        {
            try
            {
                System.Threading.Thread.Sleep(500);
                if (this.MonsterFight.TimeLine.CurrentFighter.ID == this.Monster.ID)
                {
                    this.MonsterFight.TimeLine.CurrentFighter.ResetPoints();
                    this.MonsterFight.TimeLine.NextPlayer();
                }
            }
            catch
            {
                this.MonsterFight.TimeLine.NextPlayer();
            }
        }

        public void CatchMove(int cell)
        {
            NextMove.Add(cell);
        }

        public void Move()
        {
            string path = Engines.Pathfinding.CreateStringPath(Monster.CellID, Monster.Dir, NextMove, MonsterFight.Map);
            MonsterFight.PlayerWantMove(Monster, path);
            System.Threading.Thread.Sleep(500 * NextMove.Count);
            NextMove.Clear();
            MonsterFight.PlayerEndMove(Monster);
        }

        public void BestSpell()
        {
            if (FirstTurn)
            {
                BoostMe();
                FirstTurn = false;
            }
            else
            {
                AttackNeightboor();
            }
        }

        public void AttackNeightboor()
        {
            int timeout = 0;
        reCast:
            if (timeout > 100)
                return;
            Fighter nearestFighter = GetNearestFighter();
            foreach (Game.Spells.WorldSpell ownSpell in Monster.Monster.OwnSpells.FindAll(x => x.Template.Engine.GetLevel(x.Level).TypeOfSpell == Enums.SpellTypeEnum.ATTACK))
            {
                if (ownSpell.Template.Engine.GetLevel(ownSpell.Level).MaxPO >= MonsterFight.Map.PathfindingMaker.GetDistanceBetween(Monster.CellID, nearestFighter.CellID))
                {
                    if (Monster.CurrentAP >= ownSpell.Template.Engine.GetLevel(ownSpell.Level).CostPA)
                    {
                        if (Monster.CanCastSpell(ownSpell.SpellID))
                        {
                            MonsterFight.CastSpell(Monster, ownSpell, ownSpell.Level, nearestFighter.CellID);
                            System.Threading.Thread.Sleep(1000);
                            timeout++;
                            goto reCast;
                        }
                    }
                }
                timeout++;
                if (timeout > 200)
                    break;
            }
        }

        public void BoostMe()
        {
            foreach (Game.Spells.WorldSpell ownSpell in Monster.Monster.OwnSpells.FindAll(x => x.Template.Engine.GetLevel(x.Level).TypeOfSpell == Enums.SpellTypeEnum.BOOST))
            {
                if (Monster.CanCastSpell(ownSpell.SpellID) && Monster.CurrentAP >= ownSpell.Template.Engine.GetLevel(ownSpell.Level).CostPA)
                {
                    MonsterFight.CastSpell(Monster, ownSpell, ownSpell.Level, Monster.CellID);
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public bool CanHit(int cellID)
        {
            Fighter nearestFighter = GetNearestFighter();
            int timeout = 100;
            foreach (Game.Spells.WorldSpell ownSpell in Monster.Monster.OwnSpells)
            {
                if (ownSpell.Template.Engine.GetLevel(ownSpell.Level).MaxPO >= MonsterFight.Map.PathfindingMaker.GetDistanceBetween(cellID, nearestFighter.CellID))
                {
                    if (Monster.CurrentAP >= ownSpell.Template.Engine.GetLevel(ownSpell.Level).CostPA)
                    {
                        return true;
                    }
                }
                timeout++;
                if (timeout > 100)
                    break; ;
            }
            return false;
        }

        public List<int> MoveFar()
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
                int nextCell = GetFarestCellForGoingToFighter(nearestFighter, baseCell, closedList);
                if (nextCell != -1)
                {
                    moves.Add(nextCell);
                    baseCell = nextCell;
                }
                mp--;
                timeout++;
                if (timeout > 100)
                    break;
            }
            return moves;
        }

        public List<int> MoveUntilCanHit(Fighter fighter)
        {
            List<int> moves = new List<int>();
            Fighter nearestFighter = fighter;
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

        public List<int> GetDynObs(int removeCell = -1)
        {
            var obs = new List<int>();
            foreach (var f in this.MonsterFight.Fighters.FindAll(x => !x.IsDead))
            {
                obs.Add(f.CellID);
            }
            obs.Remove(this.Monster.CellID);
            if (removeCell != -1) obs.Remove(removeCell);
            return obs;
        }
    }
}
