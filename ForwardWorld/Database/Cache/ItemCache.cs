using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class ItemCache
    {
        public static List<Records.ItemRecord> Cache = new List<Records.ItemRecord>();

        public static void Init()
        {
            Cache = Records.ItemRecord.FindAll().ToList();
            foreach (Records.ItemRecord item in Cache)
            {
                try
                {
                    item.Engine.Load(item.Statistiques, item.WeaponInfo);
                }
                catch (Exception e)
                {
                    Utilities.ConsoleStyle.Error("Can't load item '" + item.Name + "' : " + e.ToString());
                }
            }            
        }
    }
}
