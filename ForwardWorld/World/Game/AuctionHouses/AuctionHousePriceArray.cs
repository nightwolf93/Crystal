using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.AuctionHouses
{
    public class AuctionHousePriceArray
    {
        public Database.Records.AuctionHouseRecord AuctionHouse { get; set; }
        public int ItemID { get; set; }
        public List<AuctionHousePriceRow> Rows { get; set; }

        public AuctionHousePriceArray(Database.Records.AuctionHouseRecord auction, int itemID)
        {
            this.AuctionHouse = auction;
            this.ItemID = itemID;
        }

        public void MakeCompactPrices()
        {
            this.Rows = new List<AuctionHousePriceRow>();
            var items = this.AuctionHouse.GetItems().FindAll(x => x.ItemID == this.ItemID);
            int rowID = 1;
            foreach (var item in items)
            {
                var stock = this.GetSameStockOf(item.Stats, item.SellPrice);
                if (stock != null)
                {
                    stock.Add(item.Quantity, item);
                }
                else
                {
                    stock = new AuctionHousePriceRow() { RowID = rowID };
                    stock.Add(item.Quantity, item);
                    this.Rows.Add(stock);
                    rowID++;
                }
            }
        }

        public AuctionHousePriceRow GetSameStockOf(string stats, int price)
        {
            if (this.Rows.FindAll(x => x.HaveSameStock(stats, price)).Count > 0)
                return this.Rows.FirstOrDefault(x => x.HaveSameStock(stats, price));
            return null;
        }

        public void ShowPrices(Network.WorldClient client)
        {
            var packet = new StringBuilder("EHl");
            packet.Append(this.ItemID).Append("|");
            var items = this.Rows;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (i != 0) packet.Append("|");
                packet.Append(item.RowID).Append(";").Append(item.GetStockStats()).Append(";")
                    .Append(item.HaveThisQuantity(1) ? item.GetFirstOfQuantity(1).SellPrice.ToString() : "").Append(";")
                    .Append(item.HaveThisQuantity(10) ? item.GetFirstOfQuantity(10).SellPrice.ToString() : "").Append(";")
                    .Append(item.HaveThisQuantity(100) ? item.GetFirstOfQuantity(100).SellPrice.ToString() : "").Append(";");
            }
            client.Send(packet.ToString());
        }
    }

    public class AuctionHousePriceRow
    {
        public int RowID { get; set; }
        public Dictionary<int, List<Database.Records.AuctionHouseItemRecord>> Stocks = new Dictionary<int, List<Database.Records.AuctionHouseItemRecord>>();

        public void Add(int quantity, Database.Records.AuctionHouseItemRecord item)
        {
            if (this.Stocks.ContainsKey(quantity))
            {
                this.Stocks[quantity].Add(item);
            }
            else
            {
                this.Stocks.Add(quantity, new List<Database.Records.AuctionHouseItemRecord>() { item });
            }
        }

        public string GetStockStats()
        {
            if (this.Stocks.ContainsKey(1))
            {
                return this.Stocks[1].FirstOrDefault().Stats;
            }
            else if (this.Stocks.ContainsKey(10))
            {
                return this.Stocks[10].FirstOrDefault().Stats;
            }
            else if (this.Stocks.ContainsKey(100))
            {
                return this.Stocks[100].FirstOrDefault().Stats;
            }
            else
            {
                return "#null";
            }
        }

        public int GetSellPrice()
        {
            if (this.Stocks.ContainsKey(1))
            {
                return this.Stocks[1].FirstOrDefault().SellPrice;
            }
            else if (this.Stocks.ContainsKey(10))
            {
                return this.Stocks[10].FirstOrDefault().SellPrice;
            }
            else if (this.Stocks.ContainsKey(100))
            {
                return this.Stocks[100].FirstOrDefault().SellPrice;
            }
            else
            {
                return -1;
            }
        }

        public bool HaveThisQuantity(int quantity)
        {
            switch (quantity)
            {
                case 1: return this.Stocks.ContainsKey(1);
                case 10: return this.Stocks.ContainsKey(10);
                case 100: return this.Stocks.ContainsKey(100);
                default: return false;
            }
        }

        public Database.Records.AuctionHouseItemRecord GetFirstOfQuantity(int quantity)
        {
            switch (quantity)
            {
                case 1: return (this.HaveThisQuantity(1) ? this.Stocks[1][0] : null);
                case 10: return (this.HaveThisQuantity(10) ? this.Stocks[10][0] : null);
                case 100: return (this.HaveThisQuantity(100) ? this.Stocks[100][0] : null);
                default: return null;
            }
        }

        public bool HaveSameStock(string stats, int sellPrice)
        {
            return this.GetStockStats() == stats && this.GetSellPrice() == sellPrice;
        }
    }
}
