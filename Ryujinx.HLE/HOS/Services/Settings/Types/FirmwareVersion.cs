using Ryujinx.Common.Memory;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    public struct FirmwareVersion
    {
        public byte Major { get; set; }

        public byte Minor { get; set; }

        public byte Micro { get; set; }

        public byte Padding { get; set; }

        public byte RevisionMajor { get; set; }

        public byte RevisionMinor { get; set; }

        public ushort Padding2 { get; set; }

        public Array32<char> PlatformString { get; set; }

        public Array64<byte> VersionHash { get; set; }

        public Array24<char> DisplayVersion { get; set; }

        public Array64<char> DisplayTitleHalf1 { get; set; }

        public Array64<char> DisplayTitleHalf2 { get; set; }
    }
}
