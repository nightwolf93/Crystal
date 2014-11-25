using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("shop_items")]
    public class ShopItemRecord : ActiveRecordBase<ShopItemRecord>
    {
        [PrimaryKey("id")]
        public int ID
        {
            get;
            set;
        }

        [Property("name")]
        public string Name
        {
            get;
            set;
        }

        [Property("templateID")]
        public int TemplateID
        {
            get;
            set;
        }

        [Property("normal_price")]
        public int NormalPrice
        {
            get;
            set;
        }

        [Property("vip_price")]
        public int VipPrice
        {
            get;
            set;
        }

        [Property("vip")]
        public int Vip
        {
            get;
            set;
        }
    }
}
