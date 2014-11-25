using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Handlers
{
    public static class EmoteHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("eU", typeof(EmoteHandler).GetMethod("HandleEmoteRequest"));
        }

        public static void HandleEmoteRequest(World.Network.WorldClient client, string packet)
        {
            var emoteID = int.Parse(packet.Substring(2));
            switch (emoteID)
            {
                case 1://Sit
                    client.Action.StartAutoRegen();
                    break;
            }
            client.Send("eUK" + client.Character.ID + "|" + emoteID);
        }
    }
}
