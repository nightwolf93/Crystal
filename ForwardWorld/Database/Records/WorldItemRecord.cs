using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("world_items")]
    public class WorldItemRecord : ActiveRecordBase<WorldItemRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "Id")]
        public int ID
        {
            get;
            set;
        }

        [Property("Owner")]
        public int Owner
        {
            get;
            set;
        }

        [Property("Template")]
        public int Template
        {
            get;
            set;
        }

        [Property("Quantity")]
        public int Quantity
        {
            get;
            set;
        }

        [Property("Position")]
        public int Position
        {
            get;
            set;
        }

        [Property("Effects")]
        public string Effects
        {
            get;
            set;
        }

        public Engines.ItemEngine Engine = new Engines.ItemEngine();

        public ItemRecord GetTemplate
        {
            get
            {
                return Cache.ItemCache.Cache.First(x => x.ID == Template);
            }
        }

        public string DisplayItem
        {
            get
            {
                string item = "";
                item += ID.ToString("x");
                item += "~";
                item += Template.ToString("x");
                item += "~";
                item += Quantity.ToString("x");
                item += "~";
                item += Position == -1 ? "" : Position.ToString("x");
                item += "~" + Engine.StringEffect();
                return item;
            }
        }

        public bool IsSame(WorldItemRecord item)
        {
            if (item.Template == this.Template)
            {
                return item.Effects.Trim() == this.Effects.Trim();
            }
            else
            {
                return false;
            }
        }
    }
}
