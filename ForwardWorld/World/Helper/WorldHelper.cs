using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Helper
{
    public static class WorldHelper
    {
        public static Network.WorldClient GetClientByAccountNickName(string nickname)
        {
            foreach (Network.WorldClient client in GetClientsArray)
            {
                if (client.Account != null)
                {
                    if (client.Character != null)
                    {
                        if (client.Account.Pseudo.ToLower() == nickname.ToLower())
                        {
                            return client;
                        }
                    }
                }
            }
            return null;
        }

        public static Network.WorldClient GetClientByAccountName(string nickname)
        {
            foreach (Network.WorldClient client in GetClientsArray)
            {
                if (client.Account != null)
                {
                    if (client.Character != null)
                    {
                        if (client.Account.Username.ToLower() == nickname.ToLower())
                        {
                            return client;
                        }
                    }
                }
            }
            return null;
        }

        public static Network.WorldClient GetClientByAccount(string nickname)
        {
            foreach (Network.WorldClient client in GetClientsArray)
            {
                if (client.Account != null)
                {
                    if (client.Account.Username.ToLower() == nickname.ToLower())
                    {
                        return client;
                    }
                }
            }
            return null;
        }

        public static Network.WorldClient GetClientByCharacter(string nickname)
        {
            foreach (Network.WorldClient client in GetClientsArray)
            {
                if (client.Account != null)
                {
                    if (client.Character != null)
                    {
                        if (client.Character.Nickname.ToLower() == nickname.ToLower())
                        {
                            return client;
                        }
                    }
                }
            }
            return null;
        }

        public static Network.WorldClient GetClientByCharacter(int id)
        {
            foreach (Network.WorldClient client in GetClientsArray)
            {
                if (client.Account != null)
                {
                    if (client.Character != null)
                    {
                        if (client.Character.ID == id)
                        {
                            return client;
                        }
                    }
                }
            }
            return null;
        }

        public static World.Network.WorldClient[] GetClientsArray
        {
            get
            {
                return World.Manager.WorldManager.Server.Clients.ToArray();
            }
        }
    }
}
