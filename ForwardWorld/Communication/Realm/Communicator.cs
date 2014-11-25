using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Communication.Realm
{
    public static class Communicator
    {
        public static CommunicationServer Server;
        public static Dictionary<string, Database.Records.AccountRecord> Tickets = new Dictionary<string, Database.Records.AccountRecord>();

        public static void InitServer()
        {
            Server = new CommunicationServer(Utilities.ConfigurationManager.GetStringValue("CommunicationHost"), 
                                                        Utilities.ConfigurationManager.GetIntValue("CommunicationPort"));
        }

        public static void ReceivedKey(RealmLink link, Protocol.ForwardPacket packet)
        {
            string key = packet.Reader.ReadString();
            if (key == Utilities.ConfigurationManager.GetStringValue("SecureKey"))
            {
                Utilities.ConsoleStyle.Realm("Key match, MainRealm connected !");
                link.IsMain = true;
                Server.MainRealm = link;
                foreach (World.Network.WorldClient client in World.Helper.WorldHelper.GetClientsArray)
                {
                    if (client.Account != null)
                    {
                        Communication.Realm.Communicator.Server.MainRealm.SendMessage
                            (new Communication.Realm.Packet.PlayerConnectedMessage(client.Account.Username));
                    }
                }
            }
            else
            {
                Utilities.ConsoleStyle.Realm("Key '" + key + "' is not valid !");
                link.Close();
            }
        }

        public static void ReceivedPlayer(RealmLink link, Protocol.ForwardPacket packet)
        {
            string ticket = packet.Reader.ReadString();
            Database.Records.AccountRecord account = new Database.Records.AccountRecord()
            {
                ID = packet.Reader.ReadInt32(),
                Username = packet.Reader.ReadString(),
                Password = packet.Reader.ReadString(),
                Pseudo = packet.Reader.ReadString(),
                SecretQuestion = packet.Reader.ReadString(),
                SecretAnswer = packet.Reader.ReadString(),
                AdminLevel = packet.Reader.ReadInt32(),
                Points = packet.Reader.ReadInt32(),
                Vip = packet.Reader.ReadInt32(),
            };
            Utilities.ConsoleStyle.Realm("Account '" + account.Username + "' added to waiting ticket");
            Tickets.Add(ticket, account);
        }

        public static void ReceivedKickPlayer(RealmLink link, Protocol.ForwardPacket packet)
        {
            //string username = packet.Reader.ReadString();
            //Utilities.ConsoleStyle.Infos("Received kick player request for " + username);
            //TODO: Fix this crash
            //var player = World.Helper.WorldHelper.GetClientByAccountName(username);
            //if (player != null)
            //{
            //    try
            //    {
            //        player.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        Utilities.ConsoleStyle.Error("Can't disconnect player : " + ex.ToString());
            //    }
            //}
        }
    }
}
