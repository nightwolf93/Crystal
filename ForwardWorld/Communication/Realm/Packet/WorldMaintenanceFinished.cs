using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Communication.Realm.Packet
{
    public class WorldMaintenanceFinished : Protocol.ForwardPacket
    {
        public WorldMaintenanceFinished()
            : base(Protocol.ForwardPacketTypeEnum.WorldMaintenanceFinished)
        { }
    }
}
