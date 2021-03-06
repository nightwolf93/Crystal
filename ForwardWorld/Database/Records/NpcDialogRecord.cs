﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.ActiveRecord;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Records
{
    [ActiveRecord("npc_dialog")]
    public partial class NpcDialogRecord : ActiveRecordBase<NpcDialogRecord>
    {
        [PrimaryKey("ID")]
        public int ID
        {
            get;
            set;
        }

        [Property("Responses")]
        public string Responses
        {
            get;
            set;
        }

        [Property("Args")]
        public string Args
        {
            get;
            set;
        }
    }
}
