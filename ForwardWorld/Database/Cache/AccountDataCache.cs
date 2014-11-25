using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Database.Cache
{
    public static class AccountDataCache
    {
        public static List<Records.AccountDataRecord> Cache = new List<Records.AccountDataRecord>();

        public static void Init()
        {
            Cache = Records.AccountDataRecord.FindAll().ToList();
        }
    }
}
