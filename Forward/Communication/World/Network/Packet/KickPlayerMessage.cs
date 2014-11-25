using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    @Author : NightWolf
    @DotNet V. : 4.0
    @Safe class name : KickPlayerMessage
*/

namespace Crystal.RealmServer.Communication.World.Network.Packet
{
    public class KickPlayerMessage : Protocol.ForwardPacket
    {
        public KickPlayerMessage(string account)
            : base(Protocol.ForwardPacketTypeEnum.KickPlayerMessage)
        {
            Writer.Write(account);
        }
    }
}
