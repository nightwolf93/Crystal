using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Authentification.Network
{
    public class AuthentificationHandler
    {
        private AuthentificationClient _client;

        public AuthentificationHandler(AuthentificationClient client)
        {
            this._client = client;
        }

        public void Dispatch(string packet)
        {
            if (packet == "Af") return;
            switch (this._client.State)
            {

                case AuthentificationState.CheckVersion:
                    CheckVersion(packet);
                    break;

                case AuthentificationState.OnCheckAccount:
                    CheckAccount(packet);
                    break;

                case AuthentificationState.OnServerList:
                    if (packet.Substring(0, 2) == "Ax")
                    {
                        SendCharactersCount();
                    }
                    else if (packet.Substring(0, 2) == "AX")
                    {
                        WantGoToWorld(packet);
                    }
                    break;
            }
        }

        private void CheckVersion(string packet)
        {
            if (packet == "1.29.1")
            {
                _client.State = AuthentificationState.OnCheckAccount;
            }
            else
            {
                _client.Close();
            }
        }

        private void CheckAccount(string packet)
        {
            string[] data = packet.Split('#');
            string username = data[0];
            string password = data[1].Substring(1);
            Database.Records.AccountRecord account = Database.Records.AccountRecord.FindByUsername(username);
            if (account != null)
            {
                if (Utilities.Security.Hash.cryptPass(_client.EncrypKey, account.Password) == password)
                {
                    _client.Account = account;
                    
                    if(Communication.World.Manager.WorldCommunicator.IsConnected(account.Username))
                    {
                        Communication.World.Manager.WorldCommunicator.SendKickPlayer(account.Username);
                    }

                    if (!AuthentificationQueue.AddClient(_client))
                    {
                        SendAccountInformation();
                        _client.State = AuthentificationState.OnServerList;
                    }
                    else
                    {
                        _client.State = AuthentificationState.InQueue;
                    }
                }
                else
                {
                    _client.Send("AlEx");
                }
            }
            else
            {
                _client.Send("AlEx");
            }
        }

        public void WantGoToWorld(string packet)
        {
            int id = int.Parse(packet.Substring(2));
            string ticket = Utilities.Basic.RandomString(32);

            Communication.World.Network.WorldLink link = Communication.World.Manager.WorldCommunicator.GetLink(id);

            Communication.World.Manager.WorldCommunicator.SendPlayerToWorld
                (link, _client, ticket);

            _client.Send("AYK" + link.GameServer.Adress + ":" + link.GameServer.GamePort + ";" + ticket);
        }

        public void SendAccountInformation()
        {
            _client.Send("Ad" + _client.Account.Pseudo);
            _client.Send("Ac0");
            SendServersState();
            _client.Send("AlK" + (_client.Account.AdminLevel > 0 ? 1 : 0));
        }

        public void SendCharactersCount()
        {
            string header = "AxK" + _client.Account.SubscriptionRemainingTime + "|";
            string charactersList = "";
            foreach (Communication.World.Network.WorldLink link in Communication.World.Manager.WorldCommunicator.Links)
            {
                if (charactersList == "")
                {
                    charactersList += link.GameServer.ID + "," + 
                        Helper.AuthentificationHelper.GetCharactersCountOnThisServer(link.GameServer.ID, _client.Account.ID);
                    continue;
                }
                charactersList = string.Join("|", charactersList, link.GameServer.ID + "," + 
                        Helper.AuthentificationHelper.GetCharactersCountOnThisServer(link.GameServer.ID, _client.Account.ID));
            }
            _client.Send(header + charactersList);
        }

        public void SendServersState()
        {
            string header = "AH";
            string servers = "";
            foreach (Communication.World.Network.WorldLink link in Communication.World.Manager.WorldCommunicator.Links)
            {
                if (servers == "")
                {
                    servers = link.GameServer.ID + ";" + (int)link.State + ";" + (link.GameServer.ID * 75) + ";1";
                    continue;
                }
                if (link.GameServer.MininumLevelRequired <= this._client.Account.AdminLevel)
                {
                    servers = string.Join("|", servers, link.GameServer.ID + ";" + (int)link.State + ";" + (link.GameServer.ID * 75) + ";1");
                }
            }
            _client.Send(header + servers);
        }
    }
}
