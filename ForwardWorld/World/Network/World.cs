using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Network
{
    public static class World
    {
        private static Timer _maintenanceTimer = new Timer();
        private static Timer _saveTimer = new Timer();

        public static Timer UpdateInformationsTimer = new Timer(5000);
        public static Database.Records.ServerInfoRecord Infos { get; set; }

        public static void GoToMap(WorldClient client, int mapid, int cellid, bool FirstMap = false)
        {
            if (!FirstMap && client.Character.Map != null)
                client.Character.Map.Engine.RemovePlayer(client);

            client.Character.MapID = mapid;
            client.Character.CellID = cellid;

            if (client.Character.Map != null)
            {
                client.Character.Map.Engine.AddPlayer(client);
                client.Character.Map.Engine.ShowMap(client);
                client.State = WorldClientState.None;
            }
            else
            {
                client.Action.SystemMessage("Carte introuvable ID : <b>" + mapid + "</b>");
            }
        }

        public static void GoToMap(WorldClient client, Database.Records.MapRecords map, int cellid, bool FirstMap = false)
        {
            if (!FirstMap && client.Character.Map != null)
                client.Character.Map.Engine.RemovePlayer(client);

            client.Character.MapID = map.ID;
            client.Character.CellID = cellid;

            if (client.Character.Map != null)
            {
                client.Character.Map.Engine.AddPlayer(client);
                client.Character.Map.Engine.ShowMap(client);
                //Handlers.GameHandler.GameInformationsRequest(client, "");
            }
            else
            {
                client.Action.SystemMessage("Carte introuvable ID : <b>" + map.ID + "</b>");
            }
        }

        public static void Send(string packet)
        {
            WorldClient[] clientsToSend = Manager.WorldManager.Server.Clients.FindAll(x => x.Character != null).ToArray();
            clientsToSend.ToList().ForEach(x => x.Send(packet));
        }

        public static void SendIM(string header, string parameters = "")
        {
            WorldClient[] clientsToSend = Manager.WorldManager.Server.Clients.FindAll(x => x.Character != null).ToArray();
            clientsToSend.ToList().ForEach(x => x.SendImPacket(header, parameters));
        }

        public static void DisconnectAllPlayer()
        {
            Crystal.WorldServer.World.Manager.WorldManager.Server.Clients.FindAll(x => x.Account.AdminLevel == 0).ForEach(x => x.Close());
        }

        public static void SendNotification(string content)
        {
            Helper.WorldHelper.GetClientsArray.ToList().ForEach(x => x.Action.NotifMessage(content));
        }

        public static void SaveWorld()
        {
            try
            {
                Communication.Realm.Communicator.Server.MainRealm.SendMessage(new Communication.Realm.Packet.WorldSave());
                WorldClient[] clientsToSave = Manager.WorldManager.Server.Clients.FindAll(x => x.Character != null).ToArray();
                if (Utilities.ConfigurationManager.GetBoolValue("ShowSaveMessage"))
                    SendIM("1164");
                foreach (WorldClient client in clientsToSave)
                {
                    try
                    {
                        client.Character.SaveAndFlush();
                        client.Action.SaveContents();
                        if (client.Action.GuildMember != null)
                        {
                            client.Action.GuildMember.Save();
                        }
                        if (client.Character.Mount != null)
                        {
                            client.Character.Mount.SaveAndFlush();
                        }
                    }
                    catch (Exception e)
                    {
                        Utilities.ConsoleStyle.Error("Can't save character " + client.Character.Nickname + " : " + e.ToString());
                        client.Action.SystemMessage("Votre personnage n'as pas ete sauvegarder suite a une erreur serveur !");
                    }
                }

                //TODO: Save guilds

                System.Threading.Thread.Sleep(2500);
                if (Utilities.ConfigurationManager.GetBoolValue("ShowSaveMessage"))
                    SendIM("1165");
                Communication.Realm.Communicator.Server.MainRealm.SendMessage(new Communication.Realm.Packet.WorldSaveFinished());
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't perform save : " + e.ToString());
            }
        }

        public static void MaintenanceWorld(int time)
        {
            SendIM("115", (time / 60000).ToString() + " minutes");
            _maintenanceTimer.Interval = time;
            _maintenanceTimer.Elapsed += new ElapsedEventHandler(GoToMaintenance);
            _maintenanceTimer.Enabled = true;
            _maintenanceTimer.Start();
        }

        public static void UnMaintenanceWorld(WorldClient client)
        {
            client.Action.SystemMessage("Le serveur est de nouveau accessible aux joueurs!");
            Communication.Realm.Communicator.Server.MainRealm.SendMessage(new Communication.Realm.Packet.WorldMaintenanceFinished());
        }

        public static void GoToMaintenance(object sender, ElapsedEventArgs e)
        {
            DisconnectAllPlayer();
            Communication.Realm.Communicator.Server.MainRealm.SendMessage(new Communication.Realm.Packet.WorldMaintenance());
            _maintenanceTimer.Enabled = false;
            _maintenanceTimer.Stop();
        }

        public static void InitAutoSave(int intervall)
        {
            _saveTimer.Interval = intervall;
            _saveTimer.Enabled = true;
            _saveTimer.Elapsed += new ElapsedEventHandler(SaveWithThread);
            _saveTimer.Start();
        }

        public static void InitAutoInformationsUpdate()
        {
            try
            {
                Infos = Database.Records.ServerInfoRecord.FindAll()[0];
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't find server informations");
            }
            UpdateInformationsTimer.Enabled = true;
            UpdateInformationsTimer.Elapsed += UpdateInformationsTimer_Elapsed;
            UpdateInformationsTimer.Start();
        }

        private static void UpdateInformationsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (Infos != null)
                {
                    Infos.Uptime = Utilities.Basic.GetUptime();
                    Infos.PlayersLogged = Helper.WorldHelper.GetClientsArray.Length;
                    Infos.Save();
                }
            }
            catch (Exception ex)
            {
                Utilities.ConsoleStyle.Error("Can't update informations : " + ex.ToString());
            }
        }

        public static void SaveWithThread(object sender, ElapsedEventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(SaveWorld));
            t.Start();
        }

    }
}
