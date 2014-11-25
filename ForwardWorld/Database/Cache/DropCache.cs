using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : DropCache
*/

namespace Crystal.WorldServer.Database.Cache
{
    public static class DropCache
    {
        public static List<Records.DropRecord> Cache = new List<Records.DropRecord>();

        public static void Init()
        {
            Cache = Records.DropRecord.FindAll().ToList();
            MonstersTemplateCache.Cache.ForEach(x => x.MonsterDrops.Clear());
            foreach (var drop in Cache)
            {
                var monster = World.Helper.MonsterHelper.GetMonsterTemplate(drop.MonsterID);
                if (monster != null)
                {
                    monster.MonsterDrops.Add(drop);
                }
            }
        }
    }
}
