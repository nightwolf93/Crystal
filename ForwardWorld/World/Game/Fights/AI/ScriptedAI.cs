using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Interop.PythonScripting;
using Crystal.WorldServer.Engines.Pathfinder;

namespace Crystal.WorldServer.World.Game.Fights.AI
{
    public class ScriptedAI : MonsterAI
    {
        public PyScript Script { get; set; }
        public Dictionary<string, string> Tags = new Dictionary<string, string>();

        public ScriptedAI(Fighter monster)
            : base(monster)
        {
        }

        public override void StartIA()
        {
            this.Script.CallPerformAI(this.Monster);
            this.EndTurn();
        }

        public void Sleep(int seconds)
        {
            System.Threading.Thread.Sleep(seconds);
        }

        public void AddTag(string tag, string value)
        {
            if(!HasTag(tag))
                this.Tags.Add(tag, value);
        }

        public string GetTag(string tag)
        {
            if (HasTag(tag))
                return this.Tags[tag];

            return "null";
        }

        public void DeleteTag(string tag)
        {
            if (HasTag(tag))
                this.Tags.Remove(tag);
        }

        public void SetTag(string tag, string value)
        {
            if (HasTag(tag))
                this.Tags[tag] = value;
        }

        public bool HasTag(string tag)
        {
            return this.Tags.ContainsKey(tag);
        }

        public bool HaveSpell(int spellID)
        {
            foreach (var spell in this.Monster.Monster.OwnSpells)
            {
                if (spell.SpellID == spellID)
                {
                    return true;
                }
            }
            return false;
        }

        public void MoveNeightboor()
        {
            this.NextMove = this.BestMoves();
            this.Move();
        }

        public void MoveNeightboorUntilCanHit()
        {
            this.NextMove = this.BestMoves();
            this.Move();
        }

        public void MoveNeightboorFriendly()
        {
            this.NextMove = this.MoveUntilCanHit(this.GetNearestFriendlyFighter());
            this.Move();
        }

        public void MoveTo(Fighter fighter)
        {
            this.NextMove = this.MoveUntilCanHit(fighter);
            this.Move();
        }

        public void Heal(int life)
        {
            this.Monster.Heal(this.Monster.ID, life, 0);
        }

        public int GetLifePercentage()
        {
            return this.Monster.LifePercentage;
        }

        public void ResetCooldowns()
        {
            this.Monster.Cooldowns.Clear();
        }

        public bool CanReachAttack(int spellID, int cellID)
        {
            var spell = this.Monster.Monster.OwnSpells.FirstOrDefault(x => x.SpellID == spellID);
            return spell.Template.Engine.GetLevel(spell.Level).MaxPO >= MonsterFight.Map.PathfindingMaker.GetDistanceBetween(Monster.CellID, cellID);
        }

        public void FightMessage(string message)
        {
            message = message.Replace("@name@", this.Monster.Nickname);
            foreach (var fighter in this.Monster.Team.Fight.Fighters)
            {
                if (fighter.IsHuman)
                {
                    fighter.Client.Action.SystemMessage(message);
                }
            }
        }

        public bool IsInCooldown(int spellID)
        {
            return !this.Monster.CanCastSpell(spellID);
        }

        public List<int> BestMoves(bool far = false)
        {
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

        public void CastMax(int spellID, int cellID)
        {
            var ok = true;
            while (ok)
            {
                ok = this.Cast(spellID, cellID);
            }
        }

        public bool Cast(int spellID, int cellID)
        {
            if (HaveSpell(spellID))
            {
                if (this.Monster.CanCastSpell(spellID))
                {
                    var spell = this.Monster.Monster.OwnSpells.FirstOrDefault(x => x.SpellID == spellID);
                    if (spell.Template.Engine.GetLevel(spell.Level).MaxPO >= MonsterFight.Map.PathfindingMaker.GetDistanceBetween(Monster.CellID, cellID))
                    {
                        if (Monster.CurrentAP >= spell.Template.Engine.GetLevel(spell.Level).CostPA)
                        {
                            MonsterFight.CastSpell(Monster, spell, spell.Level, cellID);
                            System.Threading.Thread.Sleep(1000);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }                
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool CastOnMe(int spellID)
        {
            return this.Cast(spellID, this.Monster.CellID);
        }

        public void SetApparance(int skin, int duration)
        {
            this.Monster.AddBuff(this.Monster.ID, new Spells.Buffs.ChangeSkinBuff(skin, duration, this.Monster), 0, 0, false, null);
            this.MonsterFight.Send("GA;149;" + this.Monster.ID + ";" + this.Monster.ID + "," + skin + "," + skin + "," + duration);
        }

        public int RandomFreeJoinCell()
        {
            var cells = Engines.Pathfinding.GetJoinCell(this.Monster.CellID, this.MonsterFight.Map.Map).FindAll(x => this.MonsterFight.Map.IsAvailableCell(x));
            if (cells.Count == 0) return -1;
            return cells[Utilities.Basic.Rand(0, cells.Count - 1)];
        }
    }
}
