using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crystal.WorldServer.World.Game.Items;
using Crystal.WorldServer.World.Network;

namespace Crystal.WorldServer.World.Game.Exchange
{
    public class Bank
    {
        public static void OpenBank(WorldClient client, ItemBag bag)
        {
            var packet = "EL;";
            foreach (var item in bag.Items)
            {
                packet += "O" + item.DisplayItem + ";";
            }
            
            //Kamas
            packet += "G" + client.AccountData.Bank.Kamas;

            client.Send("ECK5");
            client.Send(packet);
            client.Action.CurrentExploredBag = bag;
        }
    }
}
