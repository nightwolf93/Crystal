using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class IncarnamTeleporterCache
    {
        public static List<Records.IncarnamTeleportRecord> Cache = new List<Records.IncarnamTeleportRecord>();

        public static void Init()
        {
            Cache = Records.IncarnamTeleportRecord.FindAll().ToList();
        }
    }
}
