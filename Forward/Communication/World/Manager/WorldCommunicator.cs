using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Communication.World.Manager
{
    public static class WorldCommunicator
    {
        public static List<Network.WorldLink> Links = new List<Network.WorldLink>();


        public static void InitConnections()
        {
            Logger.LogInfo("Init connection with multiple world ...");
            foreach (Database.Records.GameServerRecord GameServer in Database.Cache.GameServerCache.Cache)
            {
                Links.Add(new Network.WorldLink(GameServer));
            }
        }

        public static void RefreshServers()
        {
            Authentification.Helper.AuthentificationHelper.ConnectedClient.ForEach(x => x.Handler.SendServersState());
        }

        public static void SendSecureKey(Network.WorldLink link)
        {
            link.SendMessage(new Network.Packet.SecureKeyMessage(Utilities.ConfigurationManager.GetStringValue("SecureKey")));
        }

        public static void SendPlayerToWorld(Network.WorldLink link, Authentification.Network.AuthentificationClient client, string ticket)
        {
            link.SendMessage(new Network.Packet.PlayerCommingMessage(client.Account, ticket));
            System.Threading.Thread.Sleep(250);
        }

        public static void ReceivedCharacterCreated(Network.WorldLink link, Protocol.ForwardPacket packet)
        {
            Database.Records.AccountCharactersInformationsRecord character = new Database.Records.AccountCharactersInformationsRecord()
            {
                Owner = packet.Reader.ReadInt32(),
                Name = packet.Reader.ReadString(),
                Server = link.GameServer.ID,
            };
            character.SaveAndFlush();
            Database.Cache.AccountCharactersInformationsCache.Cache.Add(character);
        }

        public static void ReceivedCharacterDeleted(Network.WorldLink link, Protocol.ForwardPacket packet)
        {
            string name = packet.Reader.ReadString();
            Database.Records.AccountCharactersInformationsRecord character =
                Database.Cache.AccountCharactersInformationsCache.Cache.FirstOrDefault
                (x => x.Server == link.GameServer.ID && x.Name == name);

            Database.Cache.AccountCharactersInformationsCache.Cache.Remove(character);
            character.DeleteAndFlush();
        }

        public static Network.WorldLink GetLink(int id)
        {
            return Links.First(x => x.GameServer.ID == id);
        }

        public static void ReceivedPlayerConnected(Network.WorldLink link, Protocol.ForwardPacket packet)
        {
            string name = packet.Reader.ReadString();
            if (!link.ConnectedAccount.Contains(name))
            {
                link.ConnectedAccount.Add(name);
                Database.Records.AccountRecord account = Database.Records.AccountRecord.FindByUsername(name);
                if (account != null)
                {
                    account.Logged = 1;
                    account.SaveAndFlush();
                }
            }
            Logger.LogInfo("Player '" + name + "' connected on server '" + link.GameServer.ID + "'");
        }

        public static void ReceivedPlayerDisconnected(Network.WorldLink link, Protocol.ForwardPacket packet)
        {
            try
            {
                string name = packet.Reader.ReadString();
                if (link.ConnectedAccount.Contains(name))
                {
                    link.ConnectedAccount.Remove(name);
                    Database.Records.AccountRecord account = Database.Records.AccountRecord.FindByUsername(name);
                    if (account != null)
                    {
                        account.Logged = 0;
                        account.SaveAndFlush();
                    }
                }
                Logger.LogInfo("Player '" + name + "' disconnected on from '" + link.GameServer.ID + "'");
            }
            catch (Exception e)
            {
                Logger.LogError("Can't disconnect player : " + e.ToString());
            }
        }

        public static void ReceivedShopPointUpdate(Network.WorldLink link, Protocol.ForwardPacket packet)
        {
            try
            {
                string name = packet.Reader.ReadString();
                int points = packet.Reader.ReadInt32();
                Database.Records.AccountRecord account = Database.Records.AccountRecord.FindByUsername(name);
                if (account != null)
                {
                    account.Points = points;
                    account.SaveAndFlush();
                    Logger.LogInfo("Shop points updated for '" + name + "' !");
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Cant update shop points : " + e.ToString());
            }
        }

        public static void SendKickPlayer(string account)
        {
            if (Authentification.Helper.AuthentificationHelper.GetClient(account) != null)
            {
                Authentification.Helper.AuthentificationHelper.GetClient(account).Close();
            }
            Send(new World.Network.Packet.KickPlayerMessage(account));
        }

        public static bool IsConnected(string name)
        {
            foreach (Network.WorldLink link in Links)
            {
                if (link.ConnectedAccount.Contains(name))
                    return true;
            }

            if (Authentification.Helper.AuthentificationHelper.ConnectedClient.FindAll(x => x.Account.Username == name).Count > 0)
                return true;

            return false;
        }

        public static void Send(Protocol.ForwardPacket packet)
        {
            try
            {
                foreach (var link in Links)
                {
                    link.SendMessage(packet);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Can't send packet : " + packet + " to worlds");
            }
        }
    }
}
