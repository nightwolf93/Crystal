using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.ControlCenter.Network.Packets
{
    public class PromoteAccountPacket : ForwardPacket
    {
        public PromoteAccountPacket(string account, int level)
            : base(ForwardPacketTypeEnum.NIGHTWORLD_PromoteAccount)
        {
            base.Writer.Write(account);
            base.Writer.Write(level);
        }
    }
}
