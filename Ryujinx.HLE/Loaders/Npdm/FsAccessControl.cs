using System.IO;

namespace Ryujinx.HLE.Loaders.Npdm
{
    public class FsAccessControl
    {
        public byte Version{ get; private set; }
        
        public FsAccessFlag AccessFlag { get; private set; }

        public ulong ContentOwnerIdMin { get; private set; }

        public ulong ContentOwnerIdMax { get; private set; }

        public ulong SaveDataOwnerIdMin { get; private set; }

        public ulong SaveDataOwnerIdMax { get; private set; }

        public ulong[] ContentOwnerIds { get; private set; }

        public ulong[] SaveDataOwnerIds { get; private set; }

        public FsAccessControl(Stream stream, int offset, int size)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            BinaryReader reader = new BinaryReader(stream);

            Version = reader.ReadByte();

            var contentOwnerIdCount = reader.ReadByte();
            var saveDataOwnerIdCount = reader.ReadByte();
            reader.ReadByte(); // Padding

            AccessFlag = (FsAccessFlag)reader.ReadInt64();

            ContentOwnerIdMin = reader.ReadUInt64();
            ContentOwnerIdMax = reader.ReadUInt64();

            SaveDataOwnerIdMin = reader.ReadUInt64();
            SaveDataOwnerIdMax = reader.ReadUInt64();

            /* TODO: use this only on certain versions?
            ContentOwnerIds = new ulong[contentOwnerIdCount];
            for (var i = 0; i < contentOwnerIdCount; i++)
            {
                ContentOwnerIds[i] = reader.ReadUInt64();
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
