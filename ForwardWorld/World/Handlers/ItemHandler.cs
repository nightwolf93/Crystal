using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Handlers
{
    public static class ItemHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("Od", typeof(ItemHandler).GetMethod("DeleteItem"));
            Network.Dispatcher.RegisteredMethods.Add("OM", typeof(ItemHandler).GetMethod("MoveItem"));
            Network.Dispatcher.RegisteredMethods.Add("OU", typeof(ItemHandler).GetMethod("UseItem"));
            Network.Dispatcher.RegisteredMethods.Add("OD", typeof(ItemHandler).GetMethod("DropItem"));
        }

        public static void DeleteItem(World.Network.WorldClient client, string packet)
        {
            try
            {
                string[] data = packet.Substring(2).Split('|');
                var id = int.Parse(data[0]);
                int quantity = int.Parse(data[1]);
                if (quantity > 0)
                {
                    Database.Records.WorldItemRecord item = client.Character.Items.GetItem(id);
                    if (item.Position != -1)
                    {
                        item.Engine.Effects.ForEach(x => client.Character.Stats.ApplyEffect(x, true));
                        client.Character.Stats.RefreshStats();                  
                    }
                    client.Character.Items.RemoveItem(item, quantity);
                    client.Action.RefreshCharacter();
                    client.Character.RefreshItemSet();
                    client.Action.RefreshCharacterJob(true);
                }
            }
            catch { }     
        }

        public static void MoveItem(World.Network.WorldClient client, string packet)
        {
            string[] data = packet.Substring(2).Split('|');
            int id = int.Parse(data[0]);
            int pos = int.Parse(data[1]);
            Database.Records.WorldItemRecord item = client.Character.Items.GetItem(id);
            if (item != null)
            {
                if (pos == 16)
                {
                    return;
                }
                //Protection anti-cheat
                if (item.Position != -1 && pos != -1)
                {
                    return;
                }
                if (pos == -1)
                {
                    item.Engine.Effects.ForEach(x => client.Character.Stats.ApplyEffect(x, true));
                }
                else
                {
                    if (!Utilities.ConfigurationManager.GetBoolValue("EnablePetsOnMounts") && client.Character.RideMount && pos == 8)
                    {
                        client.Action.SystemMessage("Votre familier ne peut pas monter sur votre monture !");
                        return;
                    }
                    if (item.GetTemplate.Level > client.Character.Level)
                    {
                        client.Send("OAEL");
                        return;
                    }
                    if (client.Character.Items.HaveItem(item.Template))
                    {
                        if (client.Character.Items.GetItemsStuffed().FindAll(x => x.Template == item.Template).Count > 0)
                        {
                            return;
                        }
                    }
                    if (client.Character.Items.GetItemAtPos(pos) != null)
                    {
                        Database.Records.WorldItemRecord posItem = client.Character.Items.GetItemAtPos(pos);
                        posItem.Position = -1;
                        posItem.Engine.Effects.ForEach(x => client.Character.Stats.ApplyEffect(x, true));
                        client.Send("OM" + posItem.ID + "|" + posItem.Position);
                    }
                    item.Engine.Effects.ForEach(x => client.Character.Stats.ApplyEffect(x));
                }
                if (pos == -1)
                {
                    if (client.Character.Items.HaveItem(item.Template) && client.Character.Items.HaveItemWithSameEffects(item.Effects, item.ID, item.Template))
                    {
                        Database.Records.WorldItemRecord existingItem = client.Character.Items.GetItemWithSameEffects(item.Effects, item.Template, item.ID);
                        if (existingItem != null)
                        {
                            client.Character.Items.RemoveItem(item, item.Quantity);
                            existingItem.Quantity += 1;
                            client.Character.Items.RefreshQuantity(existingItem);
                        }
                    }
                    else
                    {
                        item.Position = pos;
                        client.Send("OM" + item.ID + "|" + item.Position);
                    }
                }
                else
                {
                    if (item.Quantity > 1)
                    {
                        Database.Records.WorldItemRecord newItem = client.Character.Items.DuplicateItem(item, pos);
                        item.Quantity -= 1;
                        client.Character.Items.RefreshQuantity(item);
                        newItem.Position = pos;
                        client.Send("OM" + newItem.ID + "|" + newItem.Position);
                    }
                    else
                    {
                        item.Position = pos;
                        client.Send("OM" + item.ID + "|" + item.Position);
                    }
                }
                if (client.Character.CurrentLife > client.Character.Stats.MaxLife)
                    client.Character.CurrentLife = client.Character.Stats.MaxLife;
                client.Action.RefreshCharacter();
                client.Character.RefreshItemSet();
                client.Character.Stats.RefreshStats();
                client.Action.RefreshPods();
                client.Action.RefreshCharacterJob(true);
            }
        }

        public static void UseItem(World.Network.WorldClient client, string packet)
        {
            int id = int.Parse(packet.Substring(2).Split('|')[0]);
            Database.Records.WorldItemRecord item = client.Character.Items.GetItem(id);
            Interop.Scripting.ScriptManager.CallScript("use_item", item.Template, client, item);
        }

        public static void DropItem(World.Network.WorldClient client, string packet)
        {
            if (!Utilities.ConfigurationManager.GetBoolValue("EnableDropItems"))
            {
                client.Action.SystemMessage("Impossible de jeter des objets sur ce serveur, la fonction n'est pas activer !");
                return;
            }
            string[] data = packet.Substring(2).Split('|');
            int id = int.Parse(data[0]);
            int quantity = int.Parse(data[1]);
            if (client.Character.Map.Engine.GetDroppedItem(client.Character.CellID) == null)
            {
                Database.Records.WorldItemRecord playerItem = client.Character.Items.GetItem(id);

                if (playerItem != null)
                {
                    if (quantity == playerItem.Quantity)
                    {
                        int itemPos = playerItem.Position;
                        playerItem.Position = -1;
                        //Delete the item
                        if (itemPos != -1)
                        {
                            playerItem.Engine.Effects.ForEach(x => client.Character.Stats.ApplyEffect(x, true));
                            client.Action.RefreshCharacter();
                            client.Character.Stats.RefreshStats();
                            client.Action.RefreshRoleplayEntity();
                            client.Action.RefreshPods();
                        }
                        client.Character.Items.RemoveItem(playerItem, playerItem.Quantity, false);
                        client.Character.Map.Engine.AddNewDroppedItem(playerItem, client.Character.CellID);
                    }
                    else if (quantity < playerItem.Quantity)
                    {
                        //Remove quantity and create another object stack
                        Database.Records.WorldItemRecord duplicatedItem = new Database.Records.WorldItemRecord()
                        {
                            Template = playerItem.Template,
                            Quantity = quantity,
                            Effects = playerItem.Effects,
                            Position = -1,
                        };
                        duplicatedItem.Engine.Load(duplicatedItem.Effects, duplicatedItem.GetTemplate.WeaponInfo);
                        client.Character.Items.RemoveItem(playerItem, quantity);
                        client.Character.Map.Engine.AddNewDroppedItem(duplicatedItem, client.Character.CellID);
                    }
                    client.Character.RefreshItemSet();
                    client.Action.RefreshCharacterJob(true);
                }
            }
            else
            {
                client.Send("BN");
            }
        }
    }
}
