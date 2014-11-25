using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : MountTemplateRecord
*/

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("mount_templates")]
    public class MountTemplateRecord : ActiveRecordBase<MountTemplateRecord>
    {
        [PrimaryKey(PrimaryKeyType.Increment, "ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("ScrollID")]
        public int ScrollID
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

        [Property("Effects")]
        public string Effects
        {
            get;
            set;
        }

        public Engines.MountStatsEngine Engine { get; set; }
    }
}
