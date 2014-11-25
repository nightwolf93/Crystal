using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("hotel")]
    public class HotelRecord : ActiveRecordBase<HotelRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "id")]
        public int ID
        {
            get;
            set;
        }

        [Property("name")]
        public string Name
        {
            get;
            set;
        }

        [Property("mapid")]
        public int MapID
        {
            get;
            set;
        }

        [Property("cellid")]
        public int CellID
        {
            get;
            set;
        }

        public string Owner = "";
        public string Password = "";
        public Timer LocateTimer { get; set; }

        public void UnLocate(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.Owner = "";
                this.Password = "";
                this.LocateTimer.Enabled = false;
                this.LocateTimer.Stop();
                this.LocateTimer.Close();

                var map = World.Helper.MapHelper.FindMap(this.MapID);
                if (map != null)
                {
                    lock (map.Engine.Players.CharactersOnMap)
                    {
                        foreach (var p in map.Engine.Players.CharactersOnMap.ToArray())
                        {
                            try
                            {
                                p.Action.SystemMessage("Le temps de location de la salle est terminer !");
                                p.Action.Teleport(p.Character.SaveMap, p.Character.SaveCell);
                            }
                            catch (Exception ex)
                            {
                                Utilities.ConsoleStyle.Error("Can't unlocate room player : " + ex.ToString());
                            }
                        }
                    }
                }

                World.Network.World.SendNotification("La salle '" + this.Name + "' est desormais disponible !");
            }
            catch (Exception ex)
            {
                Utilities.ConsoleStyle.Error("Can't unlocate room : " + ex.ToString());
            }
        }
    }
}
