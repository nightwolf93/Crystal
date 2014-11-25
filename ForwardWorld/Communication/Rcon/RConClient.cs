using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;
using Crystal.WorldServer.AbstractClass;
using Crystal.WorldServer.World;

namespace Crystal.WorldServer.Communication.Rcon
{
    public class RConClient : AbstractClient
    {
        public bool Logged = false;
        public string Username = "RconClient";

        public RConClient(SilverSock.SilverSocket socket)
            : base(socket) { }

        public override void DataArrival(byte[] data)
        {
            try
            {
                string packet = Encoding.ASCII.GetString(data);
                if (packet != "")
                {
                    try
                    {
                        Logger.LogDebug("Received RCON << " + packet);
                        this.Dispatch(packet);
                    }
                    catch (Exception e)
                    {
                        Utilities.ConsoleStyle.Error("Can't execute RCon : " + e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't execute RCon : " + e.ToString());
            }
        }

        public override void Disconnected()
        {
            Utilities.ConsoleStyle.Infos("RConClient disconnected !");
            RConManager.Server.Clients.Remove(this);
        }

        public void Dispatch(string packet)
        {
            switch (packet[0])
            {
                case 'L':
                    switch (packet[1])
                    {
                        case 'K':
                            HandleLoginKey(packet.Substring(2));
                            break;
                    }
                    break;

                case 'R':
                    switch (packet[1])
                    {
                        case 'C':
                            HandleCmd(packet.Substring(2));
                            break;
                    }
                    break;
            }
        }

        #region Handlers

        public void HandleLoginKey(string packet)
        {
            string[] data = packet.Split('|');
            string username = data[0];
            string key = data[1];
            if (Utilities.ConfigurationManager.GetStringValue("SecureKey") == key)
            {
                this.Username = username;
                Utilities.ConsoleStyle.Infos("RConClient '" + username + "' Logged !");
                this.Logged = true;
            }
            else
            {
                this.Close();
            }
        }

        public void HandleCmd(string cmd)
        {
            if (this.Logged)
            {
                List<string> command = cmd.Split('|').ToArray().ToList();
                switch (command[0])
                {
                    case "world":
                        switch (command[1])
                        {
                            case "kick":
                                World.Network.WorldClient kickedPlayer = World.Helper.WorldHelper.GetClientByCharacter(command[2]);
                                if (kickedPlayer != null)
                                {
                                    World.Manager.WorldManager.SendMessage("Le joueur <b>" + kickedPlayer.Character.Nickname + "</b> a ete kicker du serveur ! ");
                                    kickedPlayer.Close();
                                }
                                break;

                            case "save":
                                World.Network.World.SaveWithThread(null, null);
                                break;
                        }
                        break;

                    case "player":
                        switch (command[1])
                        {
                            case "message":
                                string sayPlayer = command[2];
                                string sayMessage = command[3];
                                var sayWPlayer = World.Helper.WorldHelper.GetClientByAccountName(sayPlayer);
                                if (sayWPlayer != null)
                                {
                                    sayWPlayer.Action.SystemMessage(sayMessage);
                                }
                                break;

                            case "level":
                                string leveledPlayer = command[2];
                                int levelEarned = int.Parse(command[3]);
                                var leveledWPlayer = World.Helper.WorldHelper.GetClientByAccountName(leveledPlayer);
                                if (leveledWPlayer != null)
                                {
                                    var floor = World.Helper.ExpFloorHelper.GetCharactersLevelFloor(leveledWPlayer.Character.Level + levelEarned);
                                    leveledWPlayer.Character.Experience = floor.Character;
                                    leveledWPlayer.Action.TryLevelUp();
                                }
                                break;

                            case "name":
                                string namedPlayer = command[2];
                                string newName = command[3];
                                var namedWPlayer = World.Helper.WorldHelper.GetClientByAccountName(namedPlayer);
                                if (namedWPlayer != null)
                                {
                                    namedWPlayer.Character.Nickname = newName;
                                    namedWPlayer.Character.SaveAndFlush();
                                    namedWPlayer.Action.RefreshRoleplayEntity();
                                    namedWPlayer.Character.Stats.RefreshStats();
                                }
                                break;
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
