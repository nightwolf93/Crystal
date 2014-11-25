using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Fights
{
    public class FightSpectator
    {
        public World.Network.WorldClient Client { get; set; }
        public Fight WatchedFight { get; set; }

        public FightSpectator(World.Network.WorldClient client, Fight fight)
        {
            this.Client = client;
            this.WatchedFight = fight;
        }

        public void Send(string packet)
        {
            this.Client.Send(packet);
        }
    }
}
