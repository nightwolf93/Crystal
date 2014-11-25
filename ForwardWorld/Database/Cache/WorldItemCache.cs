using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zivsoft.Log;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class WorldItemCache
    {
        public static List<Records.WorldItemRecord> Cache = new List<Records.WorldItemRecord>();

        public static void Init()
        {
            Cache = Records.WorldItemRecord.FindAll().ToList();
            foreach (Records.WorldItemRecord i in Cache.ToArray())
            {
                if (i.Owner == 0)
                {
                    Cache.Remove(i);
                    i.DeleteAndFlush();
                }
            }
            Utilities.ConsoleStyle.Infos("Item unused cleared !");
            foreach (Records.WorldItemRecord i in Cache)
            {
                try
                {
                    i.Engine.Load(i.Effects, i.GetTemplate.WeaponInfo);
                    CharacterCache.Cache.FirstOrDefault(x => x.ID == i.Owner).Items.AddItem(i, i.Position == -1 ? false : true, i.Quantity);
                    if (i.Position != -1)
                    {
                        i.Engine.Effects.ForEach(x => CharacterCache.Cache.FirstOrDefault(y => y.ID == i.Owner).Stats.ApplyEffect(x));
                    }
                }
                catch
                {
                    i.Owner = -1;
                }
            }
            foreach (Records.WorldItemRecord i in Cache.ToArray())
            {
                try
                {
                    if (i.Owner == 0)
                    {
                        Cache.Remove(i);
                        i.DeleteAndFlush();
                    }
                }
                catch (Exception e)
                {
                    //TODO: Error
                }
            }
        }
    }
}
