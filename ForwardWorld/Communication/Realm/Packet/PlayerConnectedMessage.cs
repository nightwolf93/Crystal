using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Communication.Realm.Packet
{
    public class PlayerConnectedMessage : Protocol.ForwardPacket
    {
        public PlayerConnectedMessage(string name)
            : base(Protocol.ForwardPacketTypeEnum.PlayerConnectedMessage)
        {
            Writer.Write(name);
        }
    }
}
