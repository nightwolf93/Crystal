using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Database.Records;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Helper
{
    public static class ItemHelper
    {
        public static Database.Records.WorldItemRecord GenerateItem(World.Network.WorldClient client, int templateID, bool canGenerateMount = true)
        {
            try
            {
                Database.Records.ItemRecord template = Database.Cache.ItemCache.Cache.First(x => x.ID == templateID);
                Database.Records.WorldItemRecord item = new Database.Records.WorldItemRecord();
                item.Template = templateID;
                item.Engine.Effects = template.Engine.GetRandomEffect();
                item.Owner = client.Character.ID;
                item.Effects = item.Engine.StringEffect();
                item.Position = -1;
                item.Quantity = 1;

                item.SaveAndFlush();

                if (canGenerateMount)
                {
                    Database.Records.MountTemplateRecord mount = Helper.MountHelper.GetMountTemplateByScrool(item.Template);
                    if (mount != null)
                    {
                        Database.Records.WorldMountRecord newWMount = Game.Mounts.MountFactory.CreateMount(mount, item.ID, client.Character.ID);
                    }
                }

                return item;
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant generate items : " + e.ToString());
                return null;
            }
        }

        public static Database.Records.WorldItemRecord GenerateItem(int templateID)
        {
            try
            {
                Database.Records.ItemRecord template = Database.Cache.ItemCache.Cache.First(x => x.ID == templateID);
                Database.Records.WorldItemRecord item = new Database.Records.WorldItemRecord();
                item.Template = templateID;
                item.Engine.Effects = template.Engine.GetRandomEffect();
                item.Owner = -1;
                item.Effects = item.Engine.StringEffect();
                item.Position = -1;
                item.Quantity = 1;

                return item;
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Cant generate items : " + e.ToString());
                return null;
            }
        }

        public static bool CanCreateStack(int templateID)
        {
            if (Helper.MountHelper.GetMountTemplateByScrool(templateID) != null)
                return false;

            return true;
        }

        public static WorldItemRecord GetWorldItem(int id)
        {
            if (Database.Cache.WorldItemCache.Cache.FindAll(x => x.ID == id).Count > 0) return Database.Cache.WorldItemCache.Cache.FirstOrDefault(x => x.ID == id);
            return null;
        }

        public static Database.Records.ItemBagRecord GetItemBag(int id)
        {
            if (Database.Cache.ItemBagCache.Cache.FindAll(x => x.ID == id).Count > 0) return Database.Cache.ItemBagCache.Cache.FirstOrDefault(x => x.ID == id);
            return null;
        }

        public static Database.Records.ItemRecord GetItemTemplate(int id)
        {
            return Database.Cache.ItemCache.Cache.First(x => x.ID == id);
        }
    }
}
