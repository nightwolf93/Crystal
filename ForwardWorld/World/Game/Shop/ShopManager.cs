using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Shop
{
    public static class ShopManager
    {
        public static Utilities.BasicLogger Logger { get; set; }

        public static void WantBuyObject(World.Network.WorldClient client, int id)
        {
            if (client.Character != null)
            {
                Database.Records.ShopItemRecord item = Helper.ShopHelper.FindShopItem(id);
                if (Communication.Realm.Communicator.Server.MainRealm != null)
                {
                    if (item != null)
                    {
                        int price = item.NormalPrice;
                        if (item.Vip == 1)
                        {
                            if (client.Account.Vip != 1)
                            {
                                client.Action.SystemMessage("Vous devez etre VIP vous acheter cette objet");
                                return;
                            }
                        }
                        if (client.Account.Points >= price)
                        {
                            Database.Records.ItemRecord template = Database.Cache.ItemCache.Cache.FirstOrDefault(x => x.ID == item.TemplateID);
                            if (template != null)
                            {
                                client.Account.Points -= price;
                                Communication.Realm.Communicator.Server.MainRealm.SendMessage(new Communication.Realm.Packet.ClientShopPointUpdateMessage(client.Account.Username, client.Account.Points));
                                Database.Records.WorldItemRecord gItem = Helper.ItemHelper.GenerateItem(client, template.ID);
                                if (Helper.ItemHelper.CanCreateStack(gItem.Template))
                                {
                                    client.Character.Items.AddItem(gItem, false, 1);
                                }
                                else
                                {
                                    client.Character.Items.AddItem(gItem, true, 1);
                                }
                                Logger.WriteLine("Le compte '" + client.Account.Username + "' a acheter un objet a " + price + " points (" + template.Name + ") a " + DateTime.Now.ToString());
                                client.Action.SystemMessage("<b>Felicitations !</b> L'objet <i>" + template.Name + "</i> est desormais dans votre inventaire ! Il vous reste <b>" + client.Account.Points + "</b> " + Utilities.ConfigurationManager.GetStringValue("ShopPointName"));
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Il vous manque <b>" + (price - client.Account.Points) + "</b> " + Utilities.ConfigurationManager.GetStringValue("ShopPointName") + " pour acheter ceci !");
                        }
                    }
                    else
                    {
                        client.Action.SystemMessage("L'achat <b>n" + id + "</b> n'existe pas !");
                    }
                }
                else
                {
                    client.Action.SystemMessage("Le serveur de connexion n'est pas disponible pour enregistrer votre achat, veuilliez essayer ulterieurement !");
                }
            }
        }
    }
}
