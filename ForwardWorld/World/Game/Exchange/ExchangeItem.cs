using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Exchange
{
    public class ExchangeItem
    {
        public Database.Records.WorldItemRecord WItem { get; set; }
        public int Quantity = 0;

        public ExchangeItem(Database.Records.WorldItemRecord item, int quantity)
        {
            this.WItem = item;
            this.Quantity = quantity;
        }

        public void Add(int quantity)
        {
            if (this.Quantity + quantity > WItem.Quantity)
            {
                return;
            }

            this.Quantity += quantity;
        }

        public bool Remove(int quantity)
        {
            if (this.Quantity - quantity < 0)
            {
                this.Quantity = 0;
                return true;
            }

            this.Quantity -= quantity;

            if (this.Quantity == 0)
            {
                return true;
            }

            return false;
        }
    }
}
