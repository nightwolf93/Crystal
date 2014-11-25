using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.ControlCenter.Network.Packets
{
    public class CrashServerPacket : ForwardPacket
    {
        public CrashServerPacket()
            : base(ForwardPacketTypeEnum.NIGHTWORLD_CrashServer) { }
    }
}
