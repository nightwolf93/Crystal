using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal.WorldServer.World.Game.Guilds
{
    public class GuildRightsConstants
    {
        public const int CAN_BOOST = 2;
        public const int CAN_SET_RIGHTS = 4;
        public const int CAN_INVITE = 8;
        public const int CAN_KICK = 16;
        public const int CAN_SET_XP = 32;
        public const int CAN_SET_RANK = 64;
        public const int CAN_USE_COLLECTOR = 128;
        public const int CAN_SET_MY_XP = 256;
        public const int CAN_COLLECT_COLLECTOR = 512;
        public const int CAN_USE_MOUNTPARK = 4096;
        public const int CAN_MODIFY_MOUNTPARK = 8192;
        public const int CAN_USE_MOUNTS = 16384;

        public static List<int> FullRights = new List<int>()
        {
            CAN_USE_MOUNTS,
            CAN_MODIFY_MOUNTPARK,
            CAN_USE_MOUNTPARK,
            CAN_COLLECT_COLLECTOR,
            CAN_SET_MY_XP,
            CAN_USE_COLLECTOR,
            CAN_SET_RANK,
            CAN_SET_XP,
            CAN_KICK,
            CAN_INVITE,
            CAN_SET_RIGHTS,
            CAN_BOOST,
        };
    }
}
