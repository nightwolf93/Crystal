using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("auction_house_items")]
    public class AuctionHouseItemRecord : ActiveRecordBase<AuctionHouseItemRecord>
    {

        #region Fields

        [PrimaryKey("ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("owner")]
        public int Owner
        {
            get;
            set;
        }

        [Property("auction_id")]
        public int AuctionID
        {
            get;
            set;
        }

        [Property("item_id")]
        public int ItemID
        {
            get;
            set;
        }

        [Property("quantity")]
        public int Quantity
        {
            get;
            set;
        }

        [Property("sell_price")]
        public int SellPrice
        {
            get;
            set;
        }

        [Property("start_time")]
        public double StartTime
        {
            get;
            set;
        }

        [Property("stats")]
        public string Stats
        {
            get;
            set;
        }

        #endregion


        #region Fields

        public string ToEML()
        {
            return new StringBuilder(this.ID.ToString())
                .Append("|").Append(this.Quantity).Append("|")
                .Append(this.ItemID).Append("|").Append(this.Stats)
                .Append("|").Append(this.SellPrice).Append("|-1")
                .ToString();
        }

        public string ToEL()
        {
            return this.ToEML().Replace("|", ";");
        }

        #endregion
    }
}
