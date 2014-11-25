using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.ControlCenter.Network
{
    public class ForwardPacket
    {
        public ForwardPacketTypeEnum ID;
        public BinaryWriter Writer;
        public BinaryReader Reader;
        public MemoryStream Stream;

        public ForwardPacket(ForwardPacketTypeEnum id)
        {
            ID = id;
            Stream = new MemoryStream();
            Writer = new BinaryWriter(Stream);
            Writer.Write((byte)id);
        }

        public ForwardPacket(byte[] data)
        {
            Stream = new MemoryStream(data);
            Reader = new BinaryReader(Stream);
            ID = (ForwardPacketTypeEnum)Reader.ReadByte();
        }

        public byte[] GetBytes
        {
            get
            {
                return Stream.ToArray();
            }
        }
    }
}
