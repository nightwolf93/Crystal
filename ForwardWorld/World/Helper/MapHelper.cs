using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Helper
{
    public static class MapHelper
    {
        public static void AssignTrigger(Database.Records.TriggerRecord trigger)
        {
            try
            {
                string[] data = trigger.NewMap.Split(',');
                trigger.NextMap = int.Parse(data[0]);
                trigger.NextCell = int.Parse(data[1]);
                if (data.Length > 2)
                    trigger.LevelRequired = int.Parse(data[2]);
                FindMap(trigger.MapID).Triggers.Add(trigger);
            }
            catch(Exception e)
            {
                //Utilities.ConsoleStyle.Error("Can't assign trigger : " + e.ToString());
            }
        }

        public static Database.Records.MapRecords FindMap(int id)
        {
            if (Database.Cache.MapCache.Cache.FindAll(x => x.ID == id).Count > 0)
            {
                return Database.Cache.MapCache.Cache.First(x => x.ID == id);
            }
            else
            {
                return null;
            }
        }

        public static Database.Records.MapRecords FindMap(int posX, int posY)
        {
            if (Database.Cache.MapCache.Cache.FindAll(x => x.PosX == posX && x.PosY == posY).Count > 0)
            {
                return Database.Cache.MapCache.Cache.First(x => x.PosX == posX && x.PosY == posY);
            }
            else
            {
                return null;
            }
        }

        public static Database.Records.OriginalBreedStartMapRecord GetOriginalBreedStartMap(int breed)
        {
            return Database.Cache.OriginalBreedStartMapCache.Cache.FirstOrDefault(x => x.ID == breed);
        }

        public static Database.Records.IncarnamTeleportRecord FindIncarnamTeleporter(int mapid)
        {
            if (Database.Cache.IncarnamTeleporterCache.Cache.FindAll(x => x.MapID == mapid).Count > 0)
            {
                return Database.Cache.IncarnamTeleporterCache.Cache.FirstOrDefault(x => x.MapID == mapid);
            }
            else
            {
                return null;
            }
        }

        public static Database.Records.GuildCreatorLocationRecord FindGuildCreator(int mapid, int cellid)
        {
            if (Database.Cache.GuildCreatorLocationCache.Cache.FindAll(x => x.MapID == mapid && x.CellID == cellid).Count > 0)
            {
                return Database.Cache.GuildCreatorLocationCache.Cache.FirstOrDefault(x => x.MapID == mapid && x.CellID == cellid);
            }
            else
            {
                return null;
            }
        }
    }
}
