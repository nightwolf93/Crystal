using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Handlers
{
    public class ExchangeHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("ER", typeof(ExchangeHandler).GetMethod("RequestExchange"));
            Network.Dispatcher.RegisteredMethods.Add("EA", typeof(ExchangeHandler).GetMethod("AcceptedExchangeWithPlayer"));
            Network.Dispatcher.RegisteredMethods.Add("EV", typeof(ExchangeHandler).GetMethod("LeaveExchange"));
            Network.Dispatcher.RegisteredMethods.Add("EB", typeof(ExchangeHandler).GetMethod("BuyObject"));
            Network.Dispatcher.RegisteredMethods.Add("ES", typeof(ExchangeHandler).GetMethod("SellObject"));
            Network.Dispatcher.RegisteredMethods.Add("EL", typeof(ExchangeHandler).GetMethod("ExchangeLoad"));
            Network.Dispatcher.RegisteredMethods.Add("EK", typeof(ExchangeHandler).GetMethod("ValidateExchange"));
            Network.Dispatcher.RegisteredMethods.Add("EMO", typeof(ExchangeHandler).GetMethod("RequestMoveItemsInExchange"));
            Network.Dispatcher.RegisteredMethods.Add("EMG", typeof(ExchangeHandler).GetMethod("RequestMoveKamasInExchange"));
            Network.Dispatcher.RegisteredMethods.Add("ErC", typeof(ExchangeHandler).GetMethod("ExchangeMountScroll"));
            Network.Dispatcher.RegisteredMethods.Add("Erc", typeof(ExchangeHandler).GetMethod("ExchangeMountPaddock"));
            Network.Dispatcher.RegisteredMethods.Add("Erg", typeof(ExchangeHandler).GetMethod("StuffMountByPaddock"));
            Network.Dispatcher.RegisteredMethods.Add("Erp", typeof(ExchangeHandler).GetMethod("UnStuffMountByPaddock"));
            Network.Dispatcher.RegisteredMethods.Add("EHT", typeof(ExchangeHandler).GetMethod("GetAuctionItemsByType"));
            Network.Dispatcher.RegisteredMethods.Add("EHl", typeof(ExchangeHandler).GetMethod("GetAuctionItemPrices"));
            Network.Dispatcher.RegisteredMethods.Add("EHB", typeof(ExchangeHandler).GetMethod("BuyAuctionItem"));
        }

        public static void RequestExchange(Network.WorldClient client, string packet)
        {
            string[] data = packet.Substring(2).Split('|');
            Enums.ExchangeTypeEnum type = (Enums.ExchangeTypeEnum)int.Parse(data[0]);
            switch (type)
            {
                case Enums.ExchangeTypeEnum.NPC:
                    RequestExchangeWithNpc(client, int.Parse(data[1]));
                    break;

                case Enums.ExchangeTypeEnum.PLAYER:
                    RequestExchangeWithPlayer(client, int.Parse(data[1]));
                    break;

                case Enums.ExchangeTypeEnum.SELL_HDV:
                    RequestExchangeWithSellHDV(client, int.Parse(data[1]));
                    break;

                case Enums.ExchangeTypeEnum.HDV:
                    RequestExchangeWithHDV(client, int.Parse(data[1]));
                    break;
            }
        }

        public static void RequestExchangeWithSellHDV(Network.WorldClient client, int p)
        {
            Database.Records.NpcPositionRecord npc = client.Character.Map.Engine.GetNpc(p);
            if (npc != null && (!client.Action.IsOccuped || client.Action.CurrentAuctionHouse != null))
            {
                client.Action.CurrentAuctionItem = null;
                var ah = Game.AuctionHouses.AuctionHouseManager.GetAuctionHouse(client.Character.MapID, npc.NpcID);
                client.Action.CurrentAuctionHouse = ah;

                client.Send("EV");

                StringBuilder packet = new StringBuilder("ECK10|");
                packet.Append("1,10,100;").Append(ah.GetStrTypeFromType()).Append(";")
                    .Append(0).Append(";").Append(ah.ItemLevelLimit).Append(";")
                    .Append(ah.ItemLimit).Append(";-1;").Append("-1");
                client.Send(packet.ToString());

                var hdvItemsPacket = new StringBuilder("EL");
                var hdvItems = ah.GetItemForOwner(client.Account.ID);
                for (int i = 0; i < hdvItems.Count; i++)
                {
                    var item = hdvItems[i];
                    if (i != 0) hdvItemsPacket.Append("|");
                    hdvItemsPacket.Append(item.ToEL());
                }
                client.Send(hdvItemsPacket.ToString());
            }
        }

        public static void RequestExchangeWithNpc(Network.WorldClient client, int id)
        {
            Database.Records.NpcPositionRecord npc = client.Character.Map.Engine.GetNpc(id);
            if (npc != null && !client.Action.IsOccuped)
            {
                client.Send("ECK0|" + npc.TempID);
                client.Send("EL" + npc.Patterns.SellPattern.ToString());
                client.State = Network.WorldClientState.OnExchangePnj;
                client.Action.ExchangeNpcID = npc.Template.ID;
            }
        }

        public static void BuyObject(Network.WorldClient client, string packet)
        {
            if (client.State == Network.WorldClientState.OnExchangePnj && client.Action.ExchangeNpcID != -1)
            {
                Database.Records.NpcRecord npc = Helper.NpcHelper.GetTemplate(client.Action.ExchangeNpcID);
                string[] data = packet.Substring(2).Split('|');
                int templateID = int.Parse(data[0]);
                int quantity = int.Parse(data[1]);
                if (npc.SaleItems.Split(',').ToList().Contains(data[0]))
                {
                    Database.Records.WorldItemRecord item = Helper.ItemHelper.GenerateItem(client, templateID);
                    if (client.Character.Kamas >= item.GetTemplate.Price * quantity)
                    {
                        if (Helper.ItemHelper.CanCreateStack(item.Template))
                        {
                            client.Character.Items.AddItem(item, false, quantity);
                        }
                        else
                        {
                            client.Character.Items.AddItem(item, true, quantity);
                        }
                        client.Action.RemoveKamas(item.GetTemplate.Price * quantity);
                    }              
                }
            }
        }

        public static void SellObject(Network.WorldClient client, string packet)
        {
            if (client.State == Network.WorldClientState.OnExchangePnj && client.Action.ExchangeNpcID != -1)
            {
                string[] data = packet.Substring(2).Split('|');
                int quantity = int.Parse(data[1]);
                Database.Records.WorldItemRecord item = client.Character.Items.GetItem(int.Parse(data[0]));
                if (item != null)
                {
                    if (item.Quantity >= quantity)
                    {
                        client.Character.Items.RemoveItem(item, quantity);
                        client.Action.AddKamas(Math.Abs(quantity * (item.GetTemplate.Price / 10)));
                    }
                }
            }
        }

        public static void RequestMoveItemsInExchange(Network.WorldClient client, string packet)
        {
            if (client.Action.CurrentExchange != null)
            {
                client.Action.CurrentExchange.MoveItems(client, packet);
            }
            else if (client.Action.CurrentJobCraftSkill != null)
            {
                client.Action.CurrentJobCraftSkill.MoveCraftItem(client, packet);
            }
            else if (client.Action.CurrentExploredBag != null)
            {
                client.Action.CurrentExploredBag.HandleMoveItem(client, packet);
            }
            else if (client.Action.CurrentAuctionHouse != null)
            {
                client.Action.CurrentAuctionHouse.HandleMoveItem(client, packet);
            }
        }

        public static void RequestMoveKamasInExchange(Network.WorldClient client, string packet)
        {
            if (client.Action.CurrentExchange != null)
            {
                client.Action.CurrentExchange.MoveKamas(client, int.Parse(packet.Substring(3)));
            }
            else if (client.Action.CurrentExploredBag != null)
            {
                client.Action.CurrentExploredBag.HandleMoveKamas(client, packet.Substring(3));
            }
        }

        public static void ValidateExchange(Network.WorldClient client, string packet)
        {
            if (client.Action.CurrentExchange != null)
            {
                client.Action.CurrentExchange.Validate(client);
            }
            else if (client.Action.CurrentJobCraftSkill != null)
            {
                client.Action.CurrentJobCraftSkill.Craft(client);
            }
        }

        public static void RequestExchangeWithPlayer(Network.WorldClient client, int id)
        {
            var requestedPlayer = client.Character.Map.Engine.GetClientOnMap(id);
            if (client.Action.CurrentExchange == null)
            {
                if (requestedPlayer != null)
                {
                    if (!requestedPlayer.Action.IsOccuped)
                    {
                        var exchangeInstance = new Game.Exchange.PlayerExchange(client, requestedPlayer);
                        client.Action.CurrentExchange = exchangeInstance;
                        requestedPlayer.Action.CurrentExchange = exchangeInstance;
                        exchangeInstance.Request();
                    }
                }
                else
                {
                    client.Action.SystemMessage("Le joueur demander n'existe pas ou n'est pas connecter !");
                }
            }
        }

        public static void AcceptedExchangeWithPlayer(Network.WorldClient client, string packet)
        {
            if (client.Action.CurrentExchange != null)
            {
                client.Action.CurrentExchange.Open();
            }
        }

        public static void LeaveExchange(Network.WorldClient client, string packet = "")
        {
            if (client.State == Network.WorldClientState.OnExchangePnj)
            {
                client.Send("EV");
                client.State = Network.WorldClientState.None;
                client.Action.ExchangeNpcID = -1;
            }
            else if (client.Action.CurrentExchange != null)
            {
                client.Action.CurrentExchange.Exit();
            }
            else if (client.Action.CurrentJobCraftSkill != null)
            {
                client.Action.CurrentJobCraftSkill = null;
                client.Send("EV");
            }
            else if (client.Action.CurrentExploredBag != null)
            {
                client.Action.ClearBagExplored();
                client.Send("EV");
            }
            else if (client.Action.CurrentAuctionHouse != null)
            {
                client.Action.CurrentAuctionHouse = null;
                client.Action.CurrentAuctionItem = null;
                client.Send("EV");
            }
            else
            {
                client.Send("EV");
            }
            client.Action.SaveCharacter();
            client.Action.SaveContents();
        }

        public static void ExchangeMountScroll(Network.WorldClient client, string packet)
        {
            int mountScrollID = int.Parse(packet.Substring(3));
            var scroll = client.Character.Items.GetItem(mountScrollID);
            var mount = Helper.MountHelper.GetMountByScroll(mountScrollID);
            if (scroll != null)
            {
                if (mount != null)
                {
                    if (mount.Owner == client.Character.ID)
                    {
                        mount.ScrollID = 0;
                        mount.SaveAndFlush();
                        client.Character.Items.RemoveItem(scroll, 1);
                        client.Send("Ee+" + mount.GetMountData);
                    }
                    else
                    {
                        client.Action.SystemMessage("Cette monture ne vous appartient pas !");
                    }
                }
                else
                {
                    client.Action.SystemMessage("Impossible de trouver la monture !");
                }
            }
            else
            {
                client.Action.SystemMessage("Impossible de trouver le parchemin !");
            }
        }

        public static void ExchangeMountPaddock(Network.WorldClient client, string packet)
        {
            int mountID = int.Parse(packet.Substring(3));
            var mount = Helper.MountHelper.GetMountByID(mountID);
            if (mount != null)
            {
                var mountTemplate = Helper.MountHelper.GetMountTemplateByType(mount.MountType);
                if (mountTemplate != null)
                {
                    if (mount.Owner == client.Character.ID)
                    {
                        var scroll = Helper.ItemHelper.GenerateItem(client, mountTemplate.ScrollID, false);
                        mount.ScrollID = scroll.ID;
                        mount.SaveAndFlush();
                        client.Character.Items.AddItem(scroll);
                        client.Send("Ee-" + mount.ID);
                    }
                    else
                    {
                        client.Action.SystemMessage("Cette monture ne vous appartient pas !");
                    }
                }
                else
                {
                    client.Action.SystemMessage("Template de la monture introuvable !");
                }
            }
            else
            {
                client.Action.SystemMessage("Impossible de trouver la monture !");
            }
        }

        public static void StuffMountByPaddock(Network.WorldClient client, string packet)
        {
            var mountID = int.Parse(packet.Substring(3));
            var mount = Helper.MountHelper.GetMountByID(mountID);
            if (mount != null)
            {
                if (mount.Owner == client.Character.ID)
                {
                    if (client.Character.Mount != null)
                    {
                        client.Send("Ee+" + client.Character.Mount.GetMountData);
                        client.Character.Mount = null;
                    }

                    client.Send("Ee-" + mount.ID);
                    client.Character.Mount = mount;
                    client.Character.MountID = mount.ID;

                    if (client.Character.RideMount)
                    {
                        client.Character.RideMount = false;
                        client.Action.RefreshRoleplayEntity();
                    }

                    client.Action.SendMountPanel();
                }
                else
                {
                    client.Action.SystemMessage("Cette monture ne vous appartient pas !");
                }
            }
            else
            {
                client.Action.SystemMessage("Impossible de trouver la monture !");
            }
        }

        public static void UnStuffMountByPaddock(Network.WorldClient client, string packet)
        {
            var mountID = int.Parse(packet.Substring(3));
            var mount = Helper.MountHelper.GetMountByID(mountID);
            if (mount != null)
            {
                if (mount.Owner == client.Character.ID)
                {
                    if (client.Character.Mount != null)
                    {
                        client.Send("Ee+" + client.Character.Mount.GetMountData);

                        if (client.Character.RideMount)
                        {
                            MountHandler.RideMount(client);
                            client.Action.RefreshRoleplayEntity();
                        }

                        client.Character.Mount = null;
                        client.Action.SendMountPanel();
                    }
                }
                else
                {
                    client.Action.SystemMessage("Cette monture ne vous appartient pas !");
                }
            }
            else
            {
                client.Action.SystemMessage("Impossible de trouver la monture !");
            }
        }

        public static void ExchangeLoad(Network.WorldClient client, string packet)
        {
            if (client.Action.CurrentJobCraftSkill != null)
            {
                client.Action.CurrentJobCraftSkill.Reload();
            }
        }

        private static void RequestExchangeWithHDV(Network.WorldClient client, int p)
        {
            if (!client.Action.IsOccuped && client.Character.Fighter == null)
            {
                var npc = client.Character.Map.Engine.GetNpc(p);
                if (npc != null)
                {
                    var ah = Game.AuctionHouses.AuctionHouseManager.GetAuctionHouse(client.Character.MapID, npc.NpcID);
                    if (ah != null)
                    {
                        ah.ShowPanel(client);
                    }
                }
                else
                {
                    Utilities.ConsoleStyle.Error("Can't found npc for HDV");
                }
            }
        }

        public static void GetAuctionItemsByType(Network.WorldClient client, string packet)
        {
            var id = int.Parse(packet.Substring(3));
            if (client.Action.CurrentAuctionHouse != null)
            {
                client.Action.CurrentAuctionItem = null;

                var auction = client.Action.CurrentAuctionHouse;
                var p = new StringBuilder("EHL");
                p.Append(id).Append("|");

                var items = auction.GetItemsByType(id);
                var alreadyDisplayed = new List<int>();
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (!alreadyDisplayed.Contains(item.ItemID))
                    {
                        if (i != 0) p.Append(";");
                        p.Append(item.ItemID);
                        alreadyDisplayed.Add(item.ItemID);
                    }
                }
                client.Send(p.ToString());
            }
        }

        public static void GetAuctionItemPrices(Network.WorldClient client, string packet)
        {
            var id = int.Parse(packet.Substring(3));
            if (client.Action.CurrentAuctionHouse != null)
            {
                client.Action.CurrentAuctionItem = null;

                var auction = client.Action.CurrentAuctionHouse;
                auction.ShowItemPrices(client, id);
            }
        }

        public static void BuyAuctionItem(Network.WorldClient client, string packet)
        {
            if (client.Action.CurrentAuctionHouse != null)
            {
                var auction = client.Action.CurrentAuctionHouse;
                var data = packet.Substring(3).Split('|');
                int rowID = int.Parse(data[0]);
                int amount = int.Parse(data[1]);

                auction.BuyItem(client, rowID, amount);
            }
        }
    }
}
