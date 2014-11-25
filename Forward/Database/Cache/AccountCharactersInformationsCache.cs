using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Database.Cache
{
    public class AccountCharactersInformationsCache
    {
        public static List<Records.AccountCharactersInformationsRecord> Cache = new List<Records.AccountCharactersInformationsRecord>();

        public static void Init()
        {
            Cache = Records.AccountCharactersInformationsRecord.FindAll().ToList();
        }
    }
}
