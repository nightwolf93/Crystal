using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : MountFactory
*/

namespace Crystal.WorldServer.World.Game.Mounts
{
    public static class MountFactory
    {
        public static Database.Records.WorldMountRecord CreateMount(Database.Records.MountTemplateRecord template, int scroll, int owner)
        {
            Database.Records.WorldMountRecord newMount = new Database.Records.WorldMountRecord()
            {
                ScrollID = scroll,
                Owner = owner,
                Stats = template.Engine.RefreshStatForLevel(1),
                MountType = template.ID,
                Level = Utilities.ConfigurationManager.GetIntValue("MountStartLevel"),
                Exp = 0,
                Name = Utilities.ConfigurationManager.GetStringValue("MountBaseName"),
                Ancestors = ",,,,,,,,,,,,,",
                Energy = 1000,
            };
            newMount.SaveAndFlush();
            Database.Cache.WorldMountCache.Cache.Add(newMount);
            newMount.LoadStats();
            return newMount;
        }
    }
}
