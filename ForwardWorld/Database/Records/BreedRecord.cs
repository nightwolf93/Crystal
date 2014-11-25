using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("breeds_data")]
    public class BreedRecord : ActiveRecordBase<BreedRecord>
    {
        #region Database Fields

        [PrimaryKey("Race")]
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

        [Property("StartLife")]
        public int StartLife
        {
            get;
            set;
        }

        [Property("StartPA")]
        public int StartPA
        {
            get;
            set;
        }

        [Property("StartPM")]
        public int StartPM
        {
            get;
            set;
        }

        [Property("StartInitiative")]
        public int StartInitiative
        {
            get;
            set;
        }

        [Property("StartProspection")]
        public int StartProspection
        {
            get;
            set;
        }

        [Property("WeaponBonus")]
        public string WeaponBonus
        {
            get;
            set;
        }

        [Property("Intelligence")]
        public string FireFloor
        {
            get;
            set;
        }

        [Property("Chance")]
        public string LuckFloor
        {
            get;
            set;
        }

        [Property("Agilite")]
        public string AgilityFloor
        {
            get;
            set;
        }

        [Property("Force")]
        public string StrenghtFloor
        {
            get;
            set;
        }

        [Property("Vitalite")]
        public string LifeFloor
        {
            get;
            set;
        }

        [Property("Sagesse")]
        public string WisdomFloor
        {
            get;
            set;
        }

        #endregion

        #region Fields

        public Engines.BreedFloorEngine Engine { get; set; }

        #endregion
    }
}
