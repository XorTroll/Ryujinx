using Ryujinx.Common.Memory;
using Ryujinx.HLE.Utilities;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FirmwareVersion
    {
        public byte Major { get; set; }

        public byte Minor { get; set; }

        public byte Micro { get; set; }

        public byte Padding { get; set; }

        public byte RevisionMajor { get; set; }

        public byte RevisionMinor { get; set; }

        public ushort Padding2 { get; set; }

        public Array32<byte> PlatformString { get; set; }

        public Array64<byte> VersionHash { get; set; }

        public Array24<byte> DisplayVersion { get; set; }

        public Array64<byte> DisplayTitleHalf1 { get; set; }

        public Array64<byte> DisplayTitleHalf2 { get; set; }

        public string GetPlatformString() => StringUtils.FromSpan(PlatformString.ToSpan());

        public string GetDisplayVersion() => StringUtils.FromSpan(DisplayVersion.ToSpan());
    }
}
