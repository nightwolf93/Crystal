﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Communication.World.Network.Packet
{
    public partial class SecureKeyMessage : Protocol.ForwardPacket
    {
        public SecureKeyMessage(string key)
            : base(Protocol.ForwardPacketTypeEnum.SecureKeyMessage)
        {
            Writer.Write(key);
        }
    }
}
