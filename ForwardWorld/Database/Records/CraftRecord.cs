using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("crafts")]
    public class CraftRecord : ActiveRecordBase<CraftRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "id")]
        public int ID { get; set; }

        [Property("craft")]
        public string RecipeList { get; set; }

        public Dictionary<int, int> Components = new Dictionary<int, int>();

        public void Initialize()
        {
            foreach (var c in this.RecipeList.Split(';'))
            {
                if (c != "")
                {
                    var data = c.Split('*');
                    var template = int.Parse(data[0]);
                    var quantity = int.Parse(data[1]);
                    this.Components.Add(template, quantity);
                }
            }
        }

        public bool Compare(List<World.Game.Exchange.ExchangeItem> items)
        {
            foreach (var c in items)
            {
                if (this.Components.ContainsKey(c.WItem.Template))
                {
                    var r = this.Components[c.WItem.Template];
                    if (c.Quantity != r)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
