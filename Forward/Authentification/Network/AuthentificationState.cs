﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Authentification.Network
{
    public enum AuthentificationState
    {
        CheckVersion = 0,
        OnCheckAccount = 1,
        OnServerList = 2,
        InQueue = 3,
    }
}
