using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("item_sets")]
    public class ItemSetRecord : ActiveRecordBase<ItemSetRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "ID")]
        public int ID { get; set; }

        [Property("name")]
        public string Name { get; set; }

        [Property("items")]
        public string Items { get; set; }

        [Property("bonus")]
        public string Bonus { get; set; }

        public List<int> ItemsList = new List<int>();
        public Dictionary<int, List<World.Handlers.Items.Effect>> BonusList = new Dictionary<int, List<World.Handlers.Items.Effect>>();

        public void Initialize()
        {
            foreach (var i in this.Items.Split(','))
            {
                if (i.Trim() != "") this.ItemsList.Add(int.Parse(i.Trim()));
            }

            int lvl = 2;
            foreach (var s in this.Bonus.Split(';'))
            {
                var allBonus = new List<World.Handlers.Items.Effect>();
                this.BonusList.Add(lvl, allBonus);
                if (s.Trim() != "")
                {
                    foreach (var b in s.Trim().Split(','))
                    {
                        if (b.Trim() != "")
                        {
                            try
                            {
                                var bData = b.Trim().Split(':');
                                var bonus = new World.Handlers.Items.Effect()
                                {
                                    ID = int.Parse(bData[0].Trim()),
                                };
                                bonus.Des.Fix = int.Parse(bData[1].Trim());
                                allBonus.Add(bonus);
                            }
                            catch
                            {
                                //Utilities.ConsoleStyle.Debug("Can't load effect for item set : " + this.Name);
                            }
                        }
                    }
                }
                lvl++;
            }
        }

        public string GetEffect(int itemCount)
        {
            var itemEffect = "";
            foreach (var effect in this.BonusList[itemCount])
            {
                itemEffect += effect.ToString() + ",";
            }
            return itemEffect;
        }
    }
}
