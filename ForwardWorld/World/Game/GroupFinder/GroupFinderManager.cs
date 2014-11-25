using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zivsoft.Log;

namespace Crystal.WorldServer.World.Game.GroupFinder
{
    public class GroupFinderManager
    {
        public static List<World.Network.WorldClient> RegisteredClient = new List<Network.WorldClient>();

        public static void Start()
        {
            RegisteredClient.Clear();

        }
    }
}
