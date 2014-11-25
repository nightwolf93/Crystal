using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Engines.Map
{
    public class PlayersMapEngine
    {
        private MapEngine _map;

        public List<World.Network.WorldClient> CharactersOnMap = new List<World.Network.WorldClient>();

        public PlayersMapEngine(MapEngine engine)
        {
            _map = engine;
        }

        public void ShowPlayers(World.Network.WorldClient client)
        {
            string packet = "GM";
            CharactersOnMap.ForEach(x => packet += "|+" + x.Character.Pattern.ShowCharacterOnMap);
            client.Send(packet);
        }

        public void ShowPlayer(World.Network.WorldClient client)
        {
            if(client.Character.Pattern.ShowCharacterOnMap != "")
                _map.Send("GM|+" + client.Character.Pattern.ShowCharacterOnMap);
        }

        public void HidePlayer(World.Network.WorldClient client)
        {
            _map.Send("GM|-" + client.Character.ID);
        }
    }
}
