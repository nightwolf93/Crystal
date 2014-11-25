using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Fights
{
    public class FightTimeline
    {
        private Fight _fight { get; set; }
        public List<Fighter> TimeLine = new List<Fighter>();
        public int TimeLineIndex = -1;

        public System.Timers.Timer EndTurnTimer = new System.Timers.Timer(29500);

        public FightTimeline(Fight fight)
        {
            this._fight = fight;
        }

        public void RemixTimeLine()
        {
            this.TimeLine.Clear();
            TimeLine = new List<Fighter>();
            foreach(Fighter fighter in this._fight.Fighters.FindAll(x => !x.IsInvoc).OrderBy(x => x.Initiative).Reverse())
            {
                TimeLine.Add(fighter);
                TimeLine.AddRange(fighter.GetSummonedInvocs());
            }
        }

        public void StartTimelineTasks()
        {
            this.EndTurnTimer.Elapsed += new System.Timers.ElapsedEventHandler(EndTurnTimer_Elapsed);
            this.EndTurnTimer.Enabled = true;
            this.EndTurnTimer.Start();
            this.NextPlayer();
        }

        public void EndTimeline()
        {
            this.EndTurnTimer.Stop();
            this.EndTurnTimer.Close();
            this.EndTurnTimer.Enabled = false;
            this.TimeLine.Clear();
        }

        private void EndTurnTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (EndTurnTimer.Enabled)
            {
                this.NextPlayer();
            }
        }

        private int GetFirstFighterIndex
        {
            get
            {
                int index = -1;
                foreach (var f in this.TimeLine)
                {
                    index++;
                    if (!f.IsDead)
                    {
                        break;
                    }
                }
                return index;
            }
        }

        public Fighter CurrentFighter
        {
            get
            {
                if (this.TimeLineIndex + 1 <= this.TimeLine.Count)
                {
                    return this.TimeLine[this.TimeLineIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        public void NextPlayer()
        {
            try
            {
                if (TimeLine.Count <= 0)
                {
                    this.EndTimeline();
                    return;
                }
                if (TimeLineIndex != -1)
                {
                    if (CurrentFighter != null)
                    {
                        this.CurrentFighter.ResetPoints();
                        this.CurrentFighter.CheckSpellTurnAndBuff();
                    }

                    this._fight.Send("GTF" + CurrentFighter.ID);
                    this._fight.Send("GTR" + CurrentFighter.ID);
                }

                this.TimeLineIndex++;

                if (CurrentFighter == null)
                {
                    this.TimeLineIndex = 0;
                }

                if (CurrentFighter.IsDead)
                {
                    this.NextPlayer();
                    return;
                }

                /* Restart timer */
                this.EndTurnTimer.Close();
                this.EndTurnTimer.Start();

                this.CurrentFighter.CheckWalkingGlyph();

                this._fight.RefreshPreTurn();
                this.CurrentFighter.Stats.RefreshStats();

                this._fight.Send("GTS" + CurrentFighter.ID + "|29000");

                /* If is a monster, apply AI */
                if (!this.CurrentFighter.IsHuman && !this.CurrentFighter.IsDead)
                {
                    System.Threading.Thread.Sleep(500);
                    System.Threading.Thread brainThread = new System.Threading.Thread
                        (new System.Threading.ThreadStart(this.CurrentFighter.ArtificialBrain.StartIA));
                    this.CurrentFighter.ResetPoints();
                    brainThread.Start();
                }
                else if (this.CurrentFighter.IsDead)
                {
                    this._fight.FinishTurnRequest(this.CurrentFighter);
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant execute next player : " + e.ToString());
            }     
        }
    }
}
