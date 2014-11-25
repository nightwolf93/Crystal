using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf & MonSterC²
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("accounts_data")]
    public partial class AccountDataRecord : ActiveRecordBase<AccountDataRecord>
    {
        [PrimaryKey("ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("AccountID")]
        public int AccountID
        {
            get;
            set;
        }

        [Property("NickName")]
        public string NickName
        {
            get;
            set;
        }

        [Property("FriendsGuids")]
        public string FriendsGuids
        {
            get
            {
                return string.Join(",", FriendsIDs);
            }
            set
            {
                if (value != null && value != "")
                {
                    string[] data = value.Split(',');
                    data.ToList().ForEach(x => FriendsIDs.Add(int.Parse(x)));
                }
            }
        }

        [Property("EnemiesGuids")]
        public string EnemiesGuids
        {
            get
            {
                return string.Join(",", EnemiesIDs);
            }
            set
            {
                if (value != null && value != "")
                {
                    string[] data = value.Split(',');
                    data.ToList().ForEach(x => EnemiesIDs.Add(int.Parse(x)));
                }
            }
        }

        [Property("BankID")]
        public int BankID
        {
            get;
            set;
        }


        [Property("LastIp")]
        public string LastIp
        {
            get;
            set;
        }

        public List<int> FriendsIDs = new List<int>();
        public List<int> EnemiesIDs = new List<int>();
        public World.Game.Items.ItemBag Bank { get; set; }

        public void Load()
        {
            if (World.Helper.ItemHelper.GetItemBag(this.BankID) != null)
            {
                this.Bank = World.Helper.ItemHelper.GetItemBag(this.BankID).Engine;
            }
            else
            {
                Utilities.ConsoleStyle.Error("Can't found bank for player '" + this.NickName + "'");
            }
        }
    }
}
