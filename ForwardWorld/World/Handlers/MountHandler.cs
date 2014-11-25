using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : MountHandler
*/

namespace Crystal.WorldServer.World.Handlers
{
    public static class MountHandler
    {
        public static void RegisterMethod()
        {
            Network.Dispatcher.RegisteredMethods.Add("Rr", typeof(MountHandler).GetMethod("RideMount"));
            Network.Dispatcher.RegisteredMethods.Add("Rn", typeof(MountHandler).GetMethod("RenameMount"));
        }

        public static void RideMount(World.Network.WorldClient client, string packet = "")
        {
            if (client.Character.Mount != null)
            {
                if (client.Character.RideMount)
                {
                    client.Character.RideMount = false;
                    client.Action.RefreshRoleplayEntity();
                    ApplyMountEffect(client, true);
                }
                else
                {
                    if (client.Character.Mount.Energy > 0)
                    {
                        if (!Utilities.ConfigurationManager.GetBoolValue("EnablePetsOnMounts"))
                        {
                            var pet = client.Character.Items.GetItemAtPos(8);
                            if (pet != null)
                            {
                                ItemHandler.MoveItem(client, "OM" + pet.ID + "|-1");
                                client.Action.SystemMessage("Votre familier ne peut pas monter sur votre monture !");
                            }
                        }
                        client.Character.RideMount = true;
                        if (Utilities.ConfigurationManager.GetBoolValue("MountLostEnergy"))
                        {
                            client.Character.Mount.Energy -= 10;
                        }
                        client.Action.RefreshRoleplayEntity();
                        client.Action.SendMountPanel();
                        ApplyMountEffect(client, false);
                    }
                    else
                    {
                        client.Action.SystemMessage("Votre dragodinde est trop fatiguer pour pouvoir etre monter !");
                    }
                }
            }
        }

        public static void ApplyMountEffect(World.Network.WorldClient client, bool remove)
        {
            if (client.Character.Mount != null)
            {
                if (!remove)
                {
                    //Add stats
                    foreach (var effect in client.Character.Mount.Stats)
                    {
                        client.Character.Stats.ApplyEffect(new Items.Effect() { ID = (int)effect.EffectID, Des = new Items.Des() { Fix = (int)Math.Truncate(effect.Value) } });
                    }
                }
                else
                {
                    //Remove stats
                    foreach (var effect in client.Character.Mount.Stats)
                    {
                        client.Character.Stats.ApplyEffect(new Items.Effect() { ID = (int)effect.EffectID, Des = new Items.Des() { Fix = (int)Math.Truncate(effect.Value) } }, true);
                    }
                }
                client.Character.Stats.RefreshStats();
            }
        }

        public static void RenameMount(World.Network.WorldClient client, string packet)
        {
            if (client.Character.Mount != null)
            {
                if (packet.Length < 20)
                {
                    client.Character.Mount.Name = packet.Substring(2);
                    client.Character.Mount.SaveAndFlush();
                }
                client.Action.SendMountPanel();
            }
        }

        public static void ShowPaddocksMountData(World.Network.WorldClient client)
        {
            StringBuilder packet = new StringBuilder("ECK16|");
            List<string> data = new List<string>();
            List<Database.Records.WorldMountRecord> mounts = Helper.PaddockHelper.GetMountForOwner(client.Character.ID);
            foreach (var m in mounts)
            {
                if (m.ScrollID == 0)
                {
                    if (client.Character.Mount != null)
                    {
                        if (client.Character.Mount.ID == m.ID)
                        {
                            continue;
                        }
                    }
                    data.Add(m.GetMountData);
                }      
            }
            packet.Append(string.Join(";", data));
            client.Send(packet.ToString());
        }
    }
}
