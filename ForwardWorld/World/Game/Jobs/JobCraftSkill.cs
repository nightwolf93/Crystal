using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.World.Game.Exchange;

namespace Crystal.WorldServer.World.Game.Jobs
{
    public class JobCraftSkill
    {
        public JobSkill BaseSkill { get; set; }
        public List<Exchange.ExchangeItem> Items { get; set; }
        public List<Exchange.ExchangeItem> LastRecipe { get; set; }
        public Network.WorldClient Client { get; set; }

        public JobCraftSkill(JobSkill skill, Network.WorldClient client)
        {
            this.BaseSkill = skill;
            this.Items = new List<Exchange.ExchangeItem>();
            this.Client = client;
        }

        public Job Job
        {
            get
            {
                return this.BaseSkill.BaseJob;
            }     
        }

        public Exchange.ExchangeItem GetItem(int id)
        {
            if (this.Items.FindAll(x => x.WItem.ID == id).Count > 0)
            {
                return this.Items.FirstOrDefault(x => x.WItem.ID == id);
            }
            else
            {
                return null;
            }
        }

        public bool HaveOneOfThisItem(Network.WorldClient owner, Database.Records.WorldItemRecord item)
        {
            if (this.Items.FindAll(x => x.WItem.ID == item.ID).Count > 0)
                return true;

            return false;
        }

        public ExchangeItem GetOneOfThisItem(Network.WorldClient owner, Database.Records.WorldItemRecord item)
        {
            if (this.Items.FindAll(x => x.WItem.ID == item.ID).Count > 0)
                return this.Items.FirstOrDefault(x => x.WItem.ID == item.ID);

            return null;
        }

        public void MoveCraftItem(Network.WorldClient client, string packet)
        {
            string data = packet.Substring(4);
            char typeMove = packet[3];
            string[] itemsInfos = data.Split('|');

            int itemID = int.Parse(itemsInfos[0]);
            int quantity = int.Parse(itemsInfos[1]);

            if (quantity <= 0)
            {
                return;
            }

            if (this.Items.Count > this.BaseSkill.GetJobCraftMax())
            {
                return;
            }

            var item = client.Character.Items.GetItem(itemID);

            switch (typeMove)
            {
                case '+':
                    ExchangeItem exchangedItem = null;
                    if (HaveOneOfThisItem(client, item))
                    {
                        exchangedItem = GetOneOfThisItem(client, item);
                        if (exchangedItem != null)
                        {
                            exchangedItem.Add(quantity);
                        }
                    }
                    else
                    {
                        if (quantity <= item.Quantity)
                        {
                            exchangedItem = new ExchangeItem(item, quantity);
                            this.Items.Add(exchangedItem);
                        }
                    }
                    client.Send("EMKO+" + exchangedItem.WItem.ID + "|" + exchangedItem.Quantity);
                    break;

                case '-':
                    if (HaveOneOfThisItem(client, item))
                    {
                        var removedItem = GetOneOfThisItem(client, item);
                        if (removedItem != null)
                        {
                            if (removedItem.Remove(quantity))
                            {
                                client.Send("EMKO-" + removedItem.WItem.ID);
                                this.Items.Remove(removedItem);
                            }
                            else
                            {
                                client.Send("EMKO+" + removedItem.WItem.ID + "|" + removedItem.Quantity);
                            }
                        }
                    }
                    break;
            }
        }

        public void Refresh()
        {
            foreach (var i in Items)
            {
                this.Client.Send("EMKO+" + i.WItem.ID + "|" + i.Quantity);
            }
        }

        public bool Reload()
        {
            if (this.LastRecipe != null)
            {
                this.Items.Clear();
                foreach (var i in this.LastRecipe)
                {
                    if (i.WItem.Quantity < i.Quantity)
                    {
                        return false;
                    }
                }
                this.Items = this.LastRecipe.ToArray().ToList();
                this.Refresh();
                return true;
            }
            else
            {
                return false;
            }
            //Client.Send("EcEI"); //Pas assez de ressource
        }

        public void Craft(Network.WorldClient client)
        {
            var recipe = JobHelper.GetCraft(this.BaseSkill.ID, this.Items);
            var chance = this.BaseSkill.GetCraftChance(this.Items.Count);
            var success = false;
            if (chance >= Utilities.Basic.Rand(0, 100))
            {
                success = true;
            }

            System.Threading.Thread.Sleep(750);

            if (recipe != null)
            {
                if (success)
                {
                    Database.Records.WorldItemRecord item = Helper.ItemHelper.GenerateItem(client, recipe.ID);
                    var i = client.Character.AddItem(item, 1);

                    client.Action.RefreshPods();
                    client.Send("EmKO+" + i.ID + "|1|" + recipe.ID + "|" + item.DisplayItem);
                    client.Send("EcK;" + recipe.ID);
                    client.Character.Map.Engine.Send("IO" + client.Character.ID + "|+" + recipe.ID);
                    client.Send("Ea1");
                    this.LastRecipe = this.Items.ToArray().ToList();
                }
                else
                {
                    client.Send("EcEF");
                }
            }
            else
            {
                client.Send("EcEI");
            }

            this.DeleteRecipeFromClient();
            this.Job.SendJob(client);
            this.Items.Clear();
            this.Refresh();
            this.Client.Action.SaveContents();
        }

        public void DeleteRecipeFromClient()
        {
            foreach (var i in this.Items)
            {
                this.Client.Character.Items.RemoveItem(i.WItem, i.Quantity);
            }
        }
    }
}
