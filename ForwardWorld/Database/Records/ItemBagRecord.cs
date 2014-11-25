using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("item_bags")]
    public class ItemBagRecord : ActiveRecordBase<ItemBagRecord>
    {
        public World.Game.Items.ItemBag Engine { get; set; }

        [PrimaryKey(PrimaryKeyType.Increment, "id")]
        public int ID
        {
            get;
            set;
        }

        [Property("items")]
        public string ItemsString
        {
            get
            {
                var i = new List<int>();
                foreach (var item in Engine.Items)
                {
                    i.Add(item.ID);
                }
                return string.Join(",", i);
            }
            set
            {
                var i = value.Split(',');
                foreach (var item in i)
                {
                    if (item.Trim() != "")
                    {
                        var witem = World.Helper.ItemHelper.GetWorldItem(int.Parse(item));
                        if (witem != null)
                        {
                            this.Engine.Add(witem);
                        }
                    }
                }
            }
        }

        [Property("kamas")]
        public int kamas
        {
            get
            {
                return Engine.Kamas;
            }
            set
            {
                Engine.Kamas = value;
            }
        }

        public ItemBagRecord()
        {
            this.Engine = new World.Game.Items.ItemBag(this);
            this.Engine.ID = this.ID;
        }
    }
}
