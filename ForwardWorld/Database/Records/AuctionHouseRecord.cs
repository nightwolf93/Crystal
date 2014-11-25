using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("auction_house")]
    public class AuctionHouseRecord : ActiveRecordBase<AuctionHouseRecord>
    {

        #region Fields

        [PrimaryKey("ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("npc_id")]
        public int NpcID
        {
            get;
            set;
        }

        [Property("map_id")]
        public int MapID
        {
            get;
            set;
        }

        [Property("type")]
        public int Type
        {
            get;
            set;
        }

        [Property("zone")]
        public int Zone
        {
            get;
            set;
        }

        [Property("item_limit")]
        public int ItemLimit
        {
            get;
            set;
        }

        [Property("item_level_limit")]
        public int ItemLevelLimit
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public string GetStrTypeFromType()
        {
            switch (this.Type)
            {
                case 1:
                    return "1,2,3,4";

                //TAILLEUR
                case 2:
                    return "16,17,81";

                default:
                    return "";
            }
        }

        public List<int> GetItemTypesFromTypeID()
        {
            var types = new List<int>();
            foreach (var t in this.GetStrTypeFromType().Split(','))
            {
                if (t != string.Empty)
                {
                    types.Add(int.Parse(t));
                }
            }
            return types;
        }

        public void ShowPanel(World.Network.WorldClient client)
        {
            client.Action.CurrentAuctionHouse = this;
            var packet = new StringBuilder("ECK11|");
            packet.Append("1,10,100;")
                .Append(this.GetStrTypeFromType())
                .Append(";").Append("500").Append(";")
                .Append(this.ItemLevelLimit)
                .Append(";")
                .Append(this.ItemLimit)
                .Append(";-1;").Append("2000000");
            client.Send(packet.ToString());
        }

        public void ShowItemPrices(World.Network.WorldClient client, int id)
        {
            var a = new World.Game.AuctionHouses.AuctionHousePriceArray(this, id);
            a.MakeCompactPrices();
            a.ShowPrices(client);

            client.Action.CurrentAuctionItem = a;
        }

        public void BuyItem(World.Network.WorldClient client, int rowID, int quantity)
        {
            if (quantity == 1 || quantity == 10 || quantity == 100)
            {
                var a = client.Action.CurrentAuctionItem;
                var row = a.Rows.FirstOrDefault(x => x.RowID == rowID);
                if (row.HaveThisQuantity(quantity))
                {
                    var item = row.GetFirstOfQuantity(quantity);
                    if (client.Character.Kamas >= item.SellPrice)
                    {
                        client.Action.RemoveKamas(item.SellPrice);
                        var genItem = World.Helper.ItemHelper.GenerateItem(item.ItemID);
                        genItem.Engine.Load(item.Stats, genItem.GetTemplate.WeaponInfo);
                        genItem.Owner = client.Character.ID;
                        client.Character.AddItem(genItem, item.Quantity);

                        // Add kamas to the owner bank
                        if (World.Helper.AccountHelper.ExistAccountData(item.Owner))
                        {
                            var ownerAccount = World.Helper.AccountHelper.GetAccountData(item.Owner);
                            ownerAccount.Bank.AddKamas(item.SellPrice);
                            ownerAccount.Bank.Save();
                            client.Action.SystemMessage("Vous avez acheté cette objet a : <b>" + ownerAccount.NickName + "</b>");

                            var ownerClient = World.Helper.WorldHelper.GetClientByAccountNickName(ownerAccount.NickName);
                            if (ownerClient != null)
                            {
                                ownerClient.Action.SystemMessage("Votre banque a été créditée de " + item.SellPrice + " kamas grâce a la vente de <b>" + genItem.GetTemplate.Name + "</b>");
                            }
                        }
                        else
                        {
                            client.Action.SystemMessage("Cette objet etait un objet abandonné vos kamas sont désormais dans le néant total !");
                        }

                        genItem.SaveAndFlush();
                        Database.Cache.AuctionHouseItemsCache.Cache.Remove(item);
                        item.Delete();
                        a.MakeCompactPrices();
                        this.ShowItemPrices(client, a.ItemID);
                    }
                    else
                    {
                        //TODO: Error message
                    }
                }
            }
        }

        public void HandleMoveItem(World.Network.WorldClient client, string packet)
        {
            //EMO+59|1|10000
            string data = packet.Substring(4);
            char typeMove = packet[3];
            string[] itemsInfos = data.Split('|');
            var itemID = int.Parse(itemsInfos[0]);
            var quantity = int.Parse(itemsInfos[1]);

            switch (typeMove)
            {
                case '+':                 
                    var price = int.Parse(itemsInfos[2]);
                    if (client.Character.Items.HaveItemID(itemID))
                    {
                        var item = client.Character.Items.GetItem(itemID);
                        if (item != null)
                        {
                            if (quantity > 0 && item.Quantity >= quantity)
                            {
                                var ahi = new Database.Records.AuctionHouseItemRecord()
                                {
                                    Owner = client.Account.ID,
                                    AuctionID = this.ID,
                                    ItemID = item.Template,
                                    Quantity = quantity,
                                    SellPrice = price,
                                    StartTime = 0,
                                    Stats = item.Engine.StringEffect(),
                                };
                                Database.Cache.AuctionHouseItemsCache.Cache.Add(ahi);
                                ahi.SaveAndFlush();

                                client.Character.Items.RemoveItem(item, quantity);

                                client.Send("EmK+" + ahi.ToEML());
                            }
                        }
                    }
                    break;

                case '-':
                    var ahItem = this.GetItemForOwner(client.Account.ID).FirstOrDefault(x => x.ID == itemID);
                    var genItem = World.Helper.ItemHelper.GenerateItem(ahItem.ItemID);
                    genItem.Engine.Load(ahItem.Stats, genItem.GetTemplate.WeaponInfo);
                    genItem.Owner = client.Character.ID;
                    client.Character.AddItem(genItem, ahItem.Quantity);

                    client.Send("EmK-" + ahItem.ID);

                    Database.Cache.AuctionHouseItemsCache.Cache.Remove(ahItem);
                    ahItem.DeleteAndFlush();
                    break;
            }
        }

        public List<AuctionHouseItemRecord> GetItemForOwner(int owner)
        {
            return this.GetItems().FindAll(x => x.Owner == owner);
        }

        public List<AuctionHouseItemRecord> GetItems()
        {
            var items = new List<AuctionHouseItemRecord>();
            lock (Database.Cache.AuctionHouseItemsCache.Cache) items = Database.Cache.AuctionHouseItemsCache.Cache.FindAll(x => x.AuctionID == this.ID);
            return items;
        }

        public List<AuctionHouseItemRecord> GetItemsByType(int type)
        {
            return this.GetItems().FindAll(x => World.Helper.ItemHelper.GetItemTemplate(x.ItemID).Type == type);
        }

        #endregion

    }
}
