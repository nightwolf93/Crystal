using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Communication.Realm.Packet
{
    public class WorldSaveFinished : Protocol.ForwardPacket
    {
        public WorldSaveFinished()
            : base(Protocol.ForwardPacketTypeEnum.WorldSaveFinished)
        {}
    }
}
