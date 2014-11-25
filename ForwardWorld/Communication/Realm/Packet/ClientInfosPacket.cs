using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Communication.Protocol;

namespace Crystal.WorldServer.Communication.Realm.Packet
{
    public class ClientInfosPacket : ForwardPacket
    {
        public ClientInfosPacket()
            : base(ForwardPacketTypeEnum.NIGHTWORLD_ClientInfos)
        {
            base.Writer.Write(Utilities.ConfigurationManager.GetStringValue("EmulatorName"));
            base.Writer.Write(Program.CrystalVersion);
            base.Writer.Write(Crystal.WorldServer.World.Manager.WorldManager.Server.Clients.ToArray().ToList().FindAll(x => x.Account != null).Count);
            base.Writer.Write(Utilities.Basic.GetUptime());
        }
    }
}
