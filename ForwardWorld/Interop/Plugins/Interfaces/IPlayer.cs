using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.Interop.Plugins.Interfaces
{
    public interface IPlayer
    {
        /// <summary>
        /// Send packet to the client
        /// </summary>
        /// <param name="packet">Packet</param>
        void Send(string packet);

        void APIModifyKamas(int value);
    }
}
