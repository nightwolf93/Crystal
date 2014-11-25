using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("item_db")]
    public class ItemRecord : ActiveRecordBase<ItemRecord>
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

        [Property("Type")]
        public int Type
        {
            get;
            set;
        }

        [Property("ItemSet")]
        public int ItemSet
        {
            get;
            set;
        }

        [Property("GfxId")]
        public int GfxId
        {
            get;
            set;
        }

        [Property("Level")]
        public int Level
        {
            get;
            set;
        }

        [Property("Weight")]
        public int Weight
        {
            get;
            set;
        }

        [Property("EpPropriety")]
        public int EpPropriety
        {
            get;
            set;
        }

        [Property("WeaponInfo")]
        public string WeaponInfo
        {
            get;
            set;
        }

        [Property("TwoHands")]
        public int TwoHands
        {
            get;
            set;
        }

        [Property("IsEthereal")]
        public int IsEthereal
        {
            get;
            set;
        }

        [Property("Forgemargable")]
        public int Forgemargable
        {
            get;
            set;
        }

        [Property("IsCursed")]
        public int IsCursed
        {
            get;
            set;
        }

        [Property("CanUse")]
        public int CanUse
        {
            get;
            set;
        }

        [Property("CanTarget")]
        public int CanTarget
        {
            get;
            set;
        }

        [Property("Price")]
        public int Price
        {
            get;
            set;
        }

        [Property("Condition")]
        public string Condition
        {
            get;
            set;
        }

        [Property("Statistiques")]
        public string Statistiques
        {
            get;
            set;
        }

        public Engines.ItemEngine Engine = new Engines.ItemEngine();

        public ItemSetRecord Set
        {
            get
            {
                return World.Game.Sets.ItemManager.GetSetByItem(this.ID);
            }
        }
    }
}
