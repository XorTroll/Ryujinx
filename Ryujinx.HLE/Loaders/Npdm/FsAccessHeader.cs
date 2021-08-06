using Ryujinx.HLE.Exceptions;
using System;
using System.IO;

namespace Ryujinx.HLE.Loaders.Npdm
{
    class FsAccessHeader
    {
        public byte Version { get; private set; }
        
        public FsAccessFlag AccessFlag { get; private set; }

        public uint ContentOwnerInfoOffset { get; private set; }

        public uint ContentOwnerInfoSize { get; private set; }

        public uint SaveDataOwnerInfoOffset { get; private set; }

        public uint SaveDataOwnerInfoSize { get; private set; }

        public ulong[] ContentOwnerIds { get; private set; }

        public byte[] SaveDataAccessibilities { get; private set; }

        public ulong[] SaveDataOwnerIds { get; private set; }

        public FsAccessHeader(Stream stream, int offset, int size)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            BinaryReader reader = new BinaryReader(stream);

            Version            = reader.ReadByte();
            reader.ReadBytes(3); // Padding
            AccessFlag = (FsAccessFlag)reader.ReadInt64();

            ContentOwnerInfoOffset = reader.ReadUInt32();
            ContentOwnerInfoSize = reader.ReadUInt32();

            SaveDataOwnerInfoOffset = reader.ReadUInt32();
            SaveDataOwnerInfoSize = reader.ReadUInt32();

            /* TODO: use this only on certain versions?
            var contentOwnerIdCount = reader.ReadUInt32();
            ContentOwnerIds = new ulong[contentOwnerIdCount];
            for(var i = 0; i < contentOwnerIdCount; i++)
            {
                ContentOwnerIds[i] = reader.ReadUInt64();
            }

            var saveDataOwnerIdCount = reader.ReadUInt32();
            SaveDataAccessibilities = new byte[saveDataOwnerIdCount];
            for (var i = 0; i < saveDataOwnerIdCount; i++)
            {
                SaveDataAccessibilities[i] = reader.ReadByte();
            }
            SaveDataOwnerIds = new ulong[saveDataOwnerIdCount];
            for (var i = 0; i < saveDataOwnerIdCount; i++)
            {
                SaveDataOwnerIds[i] = reader.ReadUInt64();
            }
            */
        }
    }
}
