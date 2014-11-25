using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class NpcDialogCache
    {
        public static List<Records.NpcDialogRecord> Cache = new List<Records.NpcDialogRecord>();

        public static void Init()
        {
            Cache = Records.NpcDialogRecord.FindAll().ToList();   
        }
    }
}
