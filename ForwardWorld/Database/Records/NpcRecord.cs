using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("npc_db")]
    public class NpcRecord : ActiveRecordBase<NpcRecord>
    {
        [PrimaryKey("ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("Name")]
        public string Name
        {
            get;
            set;
        }

        [Property("Gfx")]
        public int Gfx
        {
            get;
            set;
        }

        [Property("ScaleX")]
        public int ScaleX
        {
            get;
            set;
        }

        [Property("ScaleY")]
        public int ScaleY
        {
            get;
            set;
        }

        [Property("Sex")]
        public int Sex
        {
            get;
            set;
        }

        [Property("Color1")]
        public int Color1
        {
            get;
            set;
        }

        [Property("Color2")]
        public int Color2
        {
            get;
            set;
        }

        [Property("Color3")]
        public int Color3
        {
            get;
            set;
        }

        [Property("Accessories")]
        public string Accessories
        {
            get;
            set;
        }

        [Property("Clip")]
        public int Clip
        {
            get;
            set;
        }

        [Property("ArtWork")]
        public int ArtWork
        {
            get;
            set;
        }

        [Property("Bonus")]
        public int Bonus
        {
            get;
            set;
        }

        [Property("InitQuestion")]
        public int InitQuestion
        {
            get;
            set;
        }

        [Property("SaleItems")]
        public string SaleItems
        {
            get;
            set;
        }
    }
}
