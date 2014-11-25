using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.ControlCenter.Network.Packets
{
    public class ClientInfos : ForwardPacket
    {
        public ClientInfos()
            : base(ForwardPacketTypeEnum.NIGHTWORLD_ClientInfos)
        {
        }
    }
}
