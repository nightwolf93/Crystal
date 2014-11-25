using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Kolizeum
{
    public class KolizeumTeam
    {
        public List<Network.WorldClient> Clients = new List<Network.WorldClient>();

        public KolizeumTeam() { }

        public void AddMember(Network.WorldClient client)
        {
            lock (this.Clients)
            {
                this.Clients.Add(client);
            }
        }

        public bool IsFull
        {
            get
            {
                return Clients.Count >= Utilities.ConfigurationManager.GetIntValue("MemberPerTeam");
            }
        }

        public Network.WorldClient FirstFighter
        {
            get
            {
                return this.Clients.FirstOrDefault();
            }
        }

        public List<Network.WorldClient> GetClientsExceptFirst
        {
            get
            {
                var tempList = new List<Network.WorldClient>();
                tempList.AddRange(this.Clients);
                tempList.RemoveAt(0);
                return tempList;
            }
        }

        public void UnsubcribeMembers()
        {
            foreach (var client in Clients)
            {
                KolizeumManager.UnSubscribeToKolizeum(client);
            }
        }
    }
}
