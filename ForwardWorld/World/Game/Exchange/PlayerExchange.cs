using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Exchange
{
    public class PlayerExchange
    {
        public Network.WorldClient Requester { get; set; }
        public Network.WorldClient Requested { get; set; }

        public List<ExchangeItem> ItemsStack1 = new List<ExchangeItem>();
        public List<ExchangeItem> ItemsStack2 = new List<ExchangeItem>();

        public bool IsAlive = true;

        public PlayerExchange(Network.WorldClient requester, Network.WorldClient requested)
        {
            this.Requester = requester;
            this.Requested = requested;
        }

        public List<ExchangeItem> GetStackByOwner(Network.WorldClient owner)
        {
            if (Requester.Character.ID == owner.Character.ID)
            {
                return ItemsStack1;
            }
            else
            {
                return ItemsStack2;
            }
        }

        public bool HaveOneOfThisItem(Network.WorldClient owner, Database.Records.WorldItemRecord item)
        {
            var stack = GetStackByOwner(owner);

            if (stack.FindAll(x => x.WItem.ID == item.ID).Count > 0)
                return true;

            return false;
        }

        public ExchangeItem GetOneOfThisItem(Network.WorldClient owner, Database.Records.WorldItemRecord item)
        {
            var stack = GetStackByOwner(owner);

            if (stack.FindAll(x => x.WItem.ID == item.ID).Count > 0)
                return stack.FirstOrDefault(x => x.WItem.ID == item.ID);

            return null;
        }

        public Network.WorldClient GetOtherTrader(Network.WorldClient trader)
        {
            if (Requested.Character.ID == trader.Character.ID)
            {
                return Requester;
            }
            else
            {
                return Requested;
            }
        }
        
        public void Open()
        {
            if (this.IsAlive)
            {
                try
                {
                    this.Requester.Send("ECK1");
                    this.Requested.Send("ECK1");
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Can't open exchange : " + e.ToString());
                }
            }
        }

        public void Request()
        {
            try
            {
                this.Requested.Send("ERK" + this.Requester.Character.ID + "|" + this.Requested.Character.ID + "|1");
                this.Requester.Send("ERK" + this.Requester.Character.ID + "|" + this.Requested.Character.ID + "|1");
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't request exchange : " + e.ToString());
            }
        }

        public void MoveItems(Network.WorldClient client, string packet)
        {
            if (this.IsAlive)
            {
                try
                {
                    this.ResetValidate(client);
                    string data = packet.Substring(4);
                    char typeMove = packet[3];
                    string[] itemsInfos = data.Split('|');

                    int itemID = int.Parse(itemsInfos[0]);
                    int quantity = int.Parse(itemsInfos[1]);

                    if (quantity <= 0)
                    {
                        return;
                    }

                    var item = client.Character.Items.GetItem(itemID);

                    if (!ExchangeRestrictions.RestrictedItems.Contains(item.Template) || client.Account.AdminLevel > 0)
                    {
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
                                        GetStackByOwner(client).Add(exchangedItem);
                                    }
                                }
                                client.Send("EMKO+" + exchangedItem.WItem.ID + "|" + exchangedItem.Quantity);
                                GetOtherTrader(client).Send("EmKO+" + exchangedItem.WItem.ID + "|" + exchangedItem.Quantity + "|" + exchangedItem.WItem.Template + "|" + exchangedItem.WItem.Effects);
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
                                            this.GetOtherTrader(client).Send("EmKO-" + removedItem.WItem.ID);
                                            this.GetStackByOwner(client).Remove(removedItem);
                                        }
                                        else
                                        {
                                            client.Send("EMKO+" + removedItem.WItem.ID + "|" + removedItem.Quantity);
                                            GetOtherTrader(client).Send("EmKO+" + removedItem.WItem.ID + "|" + removedItem.Quantity + "|" + removedItem.WItem.Template + "|" + removedItem.WItem.Effects);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        client.Action.SystemMessage("Impossible d'echanger cette objet ! Veuilliez contacter un administrateur si vous voulez faire echanger cette objet !");
                    }
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Can't move items in exchange : " + e.ToString());
                }
            }
        }

        public void MoveKamas(Network.WorldClient client, int kamas)
        {
            if (kamas <= client.Character.Kamas && kamas >= 0)
            {
                client.Action.KamasExchangeStack = kamas;
                client.Send("EMKG" + kamas);
                this.GetOtherTrader(client).Send("EmKG" + kamas);
                this.ResetValidate(client);
            }
            else
            {
                //Anti-Cheat
                this.Exit();
            }
        }

        public void Validate(Network.WorldClient client)
        {
            if (!client.Action.ValidateExchange)
            {
                client.Action.ValidateExchange = true;
                client.Send("EK1" + client.Character.ID);
                this.GetOtherTrader(client).Send("EK1" + client.Character.ID);
                if (this.GetOtherTrader(client).Action.ValidateExchange)
                {
                    this.ProcessExchange();
                }
            }
            else
            {
                client.Action.ValidateExchange = false;
                client.Send("EK0" + client.Character.ID);
                this.GetOtherTrader(client).Send("EK0" + client.Character.ID);
            }
        }

        public void ResetValidate(Network.WorldClient client)
        {
            var trader = this.GetOtherTrader(client);
            client.Action.ValidateExchange = false;
            trader.Action.ValidateExchange = false;
            client.Send("EK0" + client.Character.ID);
            client.Send("EK0" + trader.Character.ID);
            trader.Send("EK0" + client.Character.ID);
            trader.Send("EK0" + trader.Character.ID);
        }

        public void ProcessExchange()
        {
            #region Item Process

            foreach (var item in this.ItemsStack1)
            {
                if (item.Quantity == item.WItem.Quantity)
                {
                    Requester.Character.Items.RemoveItem(item.WItem, item.Quantity, false);
                    Requested.Character.Items.AddItem(item.WItem, false, item.Quantity);
                    item.WItem.Owner = Requested.Character.ID;
                }
                else
                {
                    var newItem = new Database.Records.WorldItemRecord()
                    {
                        Owner = Requested.Character.ID,
                        Template = item.WItem.Template,
                        Quantity = item.Quantity,
                        Position = -1,
                        Effects = item.WItem.Effects,
                    };
                    Requester.Character.Items.RemoveItem(item.WItem, item.Quantity);
                    newItem.Engine.Load(newItem.Effects, newItem.GetTemplate.WeaponInfo);
                    Requested.Character.Items.AddItem(newItem);
                    newItem.SaveAndFlush();
                }
                item.WItem.SaveAndFlush();
            }

            foreach (var item in this.ItemsStack2)
            {
                if (item.Quantity == item.WItem.Quantity)
                {
                    Requested.Character.Items.RemoveItem(item.WItem, item.Quantity, false);
                    Requester.Character.Items.AddItem(item.WItem, false, item.Quantity);
                    item.WItem.Owner = Requester.Character.ID;
                }
                else
                {
                    var newItem = new Database.Records.WorldItemRecord()
                    {
                        Owner = Requester.Character.ID,
                        Template = item.WItem.Template,
                        Quantity = item.Quantity,
                        Position = -1,
                        Effects = item.WItem.Effects,
                    };
                    Requested.Character.Items.RemoveItem(item.WItem, item.Quantity);
                    newItem.Engine.Load(newItem.Effects, newItem.GetTemplate.WeaponInfo);
                    Requester.Character.Items.AddItem(newItem);
                    newItem.SaveAndFlush();
                }
                item.WItem.SaveAndFlush();
            }

            #endregion

            #region Kamas Process

            Requester.Character.Kamas -= Requester.Action.KamasExchangeStack;
            Requested.Character.Kamas += Requester.Action.KamasExchangeStack;

            Requested.Character.Kamas -= Requested.Action.KamasExchangeStack;
            Requester.Character.Kamas += Requested.Action.KamasExchangeStack;

            #endregion

            this.Requester.Send("EVa");
            this.Requested.Send("EVa");
            this.Exit(false);

            this.Requester.Action.SaveContents();
            this.Requested.Action.SaveContents();

            this.Requester.Character.Stats.RefreshStats();
            this.Requested.Character.Stats.RefreshStats();
        }

        public void Exit(bool cancel = true)
        {
            if (this.IsAlive)
            {
                try
                {
                    if (cancel)
                    {
                        this.Requester.Send("EV");
                        this.Requested.Send("EV");
                    }

                    this.IsAlive = false;

                    this.Requester.Action.KamasExchangeStack = 0;
                    this.Requested.Action.KamasExchangeStack = 0;
                    this.Requester.Action.ValidateExchange = false;
                    this.Requested.Action.ValidateExchange = false;
                    this.Requester.Action.CurrentExchange = null;
                    this.Requested.Action.CurrentExchange = null;

                    this.ItemsStack1.Clear();
                    this.ItemsStack2.Clear();
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Can't exit exchange : " + e.ToString());
                }
            }
        }
    }
}
