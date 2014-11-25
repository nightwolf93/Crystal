using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Database.Records;

namespace Crystal.WorldServer.World.Game.Items
{
    /// <summary>
    /// Bag for many virtual inventory (Banks, collector, mobs ..)
    /// 1.0.5.0 Release
    /// </summary>
    public class ItemBag
    {
        private static int m_currentId = 1;

        public static ItemBag CreateBag()
        {
            var bag = new Database.Records.ItemBagRecord();
            bag.ID = m_currentId;
            bag.Engine.Record = bag;
            m_currentId++;
            return bag.Engine;
        }

        public int ID { get; set; }
        public List<WorldItemRecord> Items = new List<WorldItemRecord>();
        public int Kamas = 0;

        public Database.Records.ItemBagRecord Record { get; set; }

        public ItemBag(ItemBagRecord record) { this.Record = record; }

        public void Create()
        {
            this.Record.CreateAndFlush();
            this.ID = this.Record.ID;
        }

        public void Add(WorldItemRecord item)
        {
            if (this.HasSame(item))
            {
                var same = this.GetSame(item);
                same.Quantity += item.Quantity;
            }
            else
            {
                this.Items.Add(item);
            }
        }

        public void AddKamas(int kamas)
        {
            this.Kamas = kamas;
            this.Record.kamas = this.Kamas;
            this.Save();
        }

        public void Add(WorldItemRecord item, int quantity)
        {
            if (this.HasSame(item))
            {
                var same = this.GetSame(item);
                same.Quantity += quantity;
                item.Quantity -= quantity;
                same.SaveAndFlush();
                item.SaveAndFlush();
            }
            else
            {
                if (item.Quantity - quantity < 0)
                {
                    item.Quantity = quantity;
                    item.SaveAndFlush();
                    this.Items.Add(item);
                }
                else
                {
                    var newItem = item.Engine.DuplicateItem(item, -1, false);
                    newItem.Owner = -1;
                    newItem.Quantity = quantity;
                    this.Items.Add(newItem);
                    newItem.SaveAndFlush();
                }
            }
        }

        public bool HasSame(WorldItemRecord item)
        {
            foreach (var i in this.Items)
            {
                if (i.IsSame(item)) return true;
            }
            return false;
        }

        public WorldItemRecord GetSame(WorldItemRecord item)
        {
            foreach (var i in this.Items)
            {
                if (i.IsSame(item)) return i;
            }
            return null;
        }

        public void Delete(bool deleteItem = true)
        {
            if (deleteItem)
            {
                foreach (var item in this.Items)
                {
                    item.DeleteAndFlush();
                }
            }
        }

        public void Save()
        {
            this.Record.SaveAndFlush();
        }

        public void HandleMoveKamas(World.Network.WorldClient client, string packet)
        {
            var kamas = 0;
            if (packet[0] == '-')//From bag
            {
                kamas = int.Parse(packet.Substring(1));
                if (this.Kamas >= kamas)//Anti-Cheat security
                {
                    client.Character.Kamas += kamas;
                    this.Kamas -= kamas;
                }
            }
            else//From character/entity
            {
                kamas = int.Parse(packet);
                if (client.Character.Kamas >= kamas)//Anti-Cheat security
                {
                    client.Character.Kamas -= kamas;
                    this.Kamas += kamas;
                }
            }
            this.Record.kamas = this.Kamas;
            this.Save();
            client.Send("ESKG" + this.Kamas);
            client.Character.Stats.RefreshStats();
        }

        public void HandleMoveItem(World.Network.WorldClient client, string packet)
        {
            try
            {
                var mode = packet[3];
                var id = int.Parse(packet.Substring(4).Split('|')[0]);
                var quantity = int.Parse(packet.Substring(4).Split('|')[1]);

                if (mode == '+')
                {
                    var item = client.Character.Items.GetItem(id);
                    if (item.Quantity >= quantity)
                    {
                        item.Quantity -= quantity;
                        if (item.Quantity == 0)
                        {
                            client.Character.Items.RemoveItem(item, quantity, false);
                            item.Owner = -1;
                            item.SaveAndFlush();
                        }
                        else
                        {
                            client.Send("OQ" + item.ID + "|" + item.Quantity);
                        }
                        this.Add(item, quantity);
                    }
                }
                else
                {
                    //TODO: Remove items
                }
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't handle move item : " + e.ToString());
            }
            finally
            {
                this.Save();
            }
        }

        public int Size()
        {
            return this.Items.Count;
        }
    }
}
