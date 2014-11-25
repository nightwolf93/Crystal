using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Communication.Realm.Packet
{
    public class ClientShopPointUpdateMessage: Protocol.ForwardPacket
    {
        public ClientShopPointUpdateMessage(string account, int points)
            : base(Protocol.ForwardPacketTypeEnum.ClientShopPointUpdateMessage)
        {
            base.Writer.Write(account);
            base.Writer.Write(points);
        }       
    }
}
