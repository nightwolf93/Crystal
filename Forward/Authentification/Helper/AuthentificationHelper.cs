using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Authentification.Helper
{
    public static class AuthentificationHelper
    {
        public static int GetCharactersCountOnThisServer(int server, int owner)
        {
            lock (Database.Cache.AccountCharactersInformationsCache.Cache)
            {
                return Database.Cache.AccountCharactersInformationsCache.Cache.FindAll(x => x.Server == server && x.Owner == owner).Count;
            }
        }

        public static List<Network.AuthentificationClient> ConnectedClient
        {
            get
            {
                lock (Manager.AuthentificationManager.Server.Clients)
                {
                    return Manager.AuthentificationManager.Server.Clients.FindAll(x => x.State == Network.AuthentificationState.OnServerList);
                }
            }
        }

        public static void ResetLogged()
        {
            foreach (Database.Records.AccountRecord account in Database.Records.AccountRecord.FindAll())
            {
                account.Logged = 0;
                account.SaveAndFlush();
            }
        }

        public static Network.AuthentificationClient GetClient(string username)
        {
            if (ConnectedClient.FindAll(x => x.Account.Username == username).Count > 0)
                return ConnectedClient.FirstOrDefault(x => x.Account.Username == username);
            return null;
        }
    }
}
