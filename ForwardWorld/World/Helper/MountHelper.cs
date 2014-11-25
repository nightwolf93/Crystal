using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : MountHelper
*/

namespace Crystal.WorldServer.World.Helper
{
    public static class MountHelper
    {
        public static Database.Records.MountTemplateRecord GetMountTemplateByScrool(int id)
        {
            if (Database.Cache.MountTemplateCache.Cache.FindAll(x => x.ScrollID == id).Count > 0)
                return Database.Cache.MountTemplateCache.Cache.FirstOrDefault(x => x.ScrollID == id);
            return null;
        }

        public static Database.Records.MountTemplateRecord GetMountTemplateByType(int id)
        {
            if (Database.Cache.MountTemplateCache.Cache.FindAll(x => x.ID == id).Count > 0)
                return Database.Cache.MountTemplateCache.Cache.FirstOrDefault(x => x.ID == id);
            return null;
        }

        public static Database.Records.WorldMountRecord GetMountByID(int id)
        {
            if (Database.Cache.WorldMountCache.Cache.FindAll(x => x.ID == id).Count > 0)
                return Database.Cache.WorldMountCache.Cache.FirstOrDefault(x => x.ID == id);
            return null;
        }

        public static Database.Records.WorldMountRecord GetMountByScroll(int id)
        {
            if (Database.Cache.WorldMountCache.Cache.FindAll(x => x.ScrollID == id).Count > 0)
                return Database.Cache.WorldMountCache.Cache.FirstOrDefault(x => x.ScrollID == id);
            return null;
        }
    }
}
