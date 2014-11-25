using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.RealmServer.Communication.Protocol
{
    public class ForwardPacket
    {
        public ForwardPacketTypeEnum ID;
        public BinaryWriter Writer;
        public BinaryReader Reader;
        public MemoryStream Stream;

        public ForwardPacket(ForwardPacketTypeEnum id)
        {
            try
            {
                ID = id;
                Stream = new MemoryStream();
                Writer = new BinaryWriter(Stream);
                Writer.Write((byte)id);
            }
            catch (Exception e)
            {

            }

        }

        public ForwardPacket(byte[] data)
        {
            try
            {
                Stream = new MemoryStream(data);
                Reader = new BinaryReader(Stream);
                ID = (ForwardPacketTypeEnum)Reader.ReadByte();
            }
            catch (Exception e)
            {

            }
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
