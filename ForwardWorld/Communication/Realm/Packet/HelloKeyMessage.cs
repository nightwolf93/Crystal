using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Communication.Realm.Packet
{
    public partial class HelloKeyMessage : Protocol.ForwardPacket
    {
        public HelloKeyMessage()
            : base(Protocol.ForwardPacketTypeEnum.HelloKeyMessage){}
    }
}
