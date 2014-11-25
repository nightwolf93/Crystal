using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.World.Network
{
    public enum WorldClientState
    {
        None = -1,
        Authentificate = 0,
        SelectCharacter = 1,
        OnDialog = 2,
        OnMove = 3,
        OnRequestZaap = 4,
        OnExchangePnj = 5,
        OnRequestIncarnamStatue = 6,
        OnRequestChallenge = 7,
        OnRequestMountDoor = 8,
    }
}
