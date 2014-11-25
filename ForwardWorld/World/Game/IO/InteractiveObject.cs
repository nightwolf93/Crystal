using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Crystal.WorldServer.World.Game.IO
{
    public class InteractiveObject
    {
        public Database.Records.MapRecords Map { get; set; }
        public Enums.InteractiveObjectEnum TypeID { get; set; }
        public int CellID { get; set; }
        public InteractiveObjectState State { get; set; }
        public bool IsInteractive { get; set; }

        private Timer _respawnTimer { get; set; }

        public InteractiveObject(Database.Records.MapRecords map, Enums.InteractiveObjectEnum typeID, int cellid, InteractiveObjectState state, bool isInteractive)
        {
            this.Map = map;
            this.TypeID = typeID;
            this.CellID = cellid;
            this.State = state;
            this.IsInteractive = isInteractive;
        }

        public void UpdateState(InteractiveObjectState state)
        {
            this.State = state;
            this.Map.Engine.Send(this.GetPattern());
        }

        public void SetFull()
        {
            this.UpdateState(InteractiveObjectState.FULLING);
            this.State = InteractiveObjectState.FULL;
        }

        public void SetEmpty()
        {
            this.UpdateState(InteractiveObjectState.EMPTY);
            this.State = InteractiveObjectState.EMPTY2;
        }

        public string GetPattern()
        {
            return "GDF|" + this.CellID + ";" + (int)this.State + ";" + (this.IsInteractive ? "1" : "0");
        }

        public Database.Records.IODataRecord IOData
        {
            get
            {
                return IO.InteractiveObjectHelper.GetIOData((int)this.TypeID);
            }
        }

        public void StartRespawnTimer()
        {
            try
            {
                var time = 200000;
                if (this.IOData != null)
                {
                    time = this.IOData.Respawn;
                    Utilities.ConsoleStyle.Debug("Starting respawning for IO '" + this.IOData.Name + "' in " + this.IOData.Respawn);
                }
                this._respawnTimer = new Timer(15000);
                this._respawnTimer.Enabled = true;
                this._respawnTimer.Elapsed += new ElapsedEventHandler(_respawnTimer_Elapsed);
                this._respawnTimer.Start();
            }
            catch { }
        }

        private void _respawnTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this._respawnTimer.Stop();
            this._respawnTimer.Close();
            this.Respawn();
        }

        public void Respawn()
        {
            Utilities.ConsoleStyle.Debug("Respawing IO '" + this.IOData.Name + "' for map " + this.Map.ID + " on cell " + this.CellID);
            this.SetFull();
        }
    }
}
