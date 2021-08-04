using Ryujinx.Common.Memory;

namespace Ryujinx.HLE.HOS.Services.Ldn.Lp2p
{
    struct GroupInfo
    {
        public Array16<byte> WrappedMasterKey { get; set; }

        public ulong LocalCommunicationId { get; set; }

        // TODO: nicer way for a 6-byte value?
        public Array6<byte> GroupId { get; set; }

        public Array33<char> ServiceName { get; set; }

        public sbyte FlagsCount { get; set; }

        public Array64<sbyte> Flags { get; set; }

        public byte SupportedPlatform { get; set; }

        public ushort Unk { get; set; }

        public ushort FrequencyGHz { get; set; }

        public ushort Channel { get; set; }

        public byte NetworkMode { get; set; }

        public byte PerformanceRequirement { get; set; }

        public byte SecurityType { get; set; }

        public byte StaticAesKeyIndex { get; set; }

        public byte Priority { get; set; }

        public byte StealthEnabled { get; set; }

        public byte Unk2 { get; set; }

        // 0x130 empty bytes
        public Array64<byte> UnkEmpty1 { get; set; }
        public Array64<byte> UnkEmpty2 { get; set; }
        public Array64<byte> UnkEmpty3 { get; set; }
        public Array64<byte> UnkEmpty4 { get; set; }
        public Array48<byte> UnkEmpty5 { get; set; }

        public byte PresharedKeyBinarySize { get; set; }

        public Array63<byte> PresharedKey { get; set; }
    }
}
