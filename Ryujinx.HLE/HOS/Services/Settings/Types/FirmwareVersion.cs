using Ryujinx.Common.Memory;
using Ryujinx.HLE.Utilities;
using System.Runtime.InteropServices;
using System;

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

        public string GetPlatformString() => StringUtils.FromArray(PlatformString);

        public string GetDisplayVersion() => StringUtils.FromArray(DisplayVersion);

        public FirmwareVersion(byte major, byte minor, byte micro)
        {
            Major = major;
            Minor = minor;
            Micro = micro;
            Padding = 0;
            RevisionMajor = 0;
            RevisionMinor = 0;
            Padding2 = 0;

            var versionStr = $"{major}.{minor}.{micro}";
            PlatformString = StringUtils.ToArray<Array32<byte>>(versionStr, 32);
            VersionHash = new Array64<byte>();
            DisplayVersion = StringUtils.ToArray<Array24<byte>>(versionStr, 24);
            DisplayTitleHalf1 = StringUtils.ToArray<Array64<byte>>(versionStr, 64);
            DisplayTitleHalf2 = new Array64<byte>();
        }

        public FirmwareVersion(Version version)
        {
            Major = (byte)version.Major;
            Minor = (byte)version.Minor;
            Micro = (byte)version.Build;
            Padding = 0;
            RevisionMajor = 0;
            RevisionMinor = 0;
            Padding2 = 0;

            var versionStr = $"{version.Major}.{version.Minor}.{version.Build}";
            PlatformString = StringUtils.ToArray<Array32<byte>>(versionStr, 32);
            VersionHash = new Array64<byte>();
            DisplayVersion = StringUtils.ToArray<Array24<byte>>(versionStr, 24);
            DisplayTitleHalf1 = StringUtils.ToArray<Array64<byte>>(versionStr, 64);
            DisplayTitleHalf2 = new Array64<byte>();
        }

        public Version Version => new Version(Major, Minor, Micro);

        public static bool operator ==(FirmwareVersion a, FirmwareVersion b)
        {
            return a.Version == b.Version;
        }

        public static bool operator !=(FirmwareVersion a, FirmwareVersion b)
        {
            return a.Version != b.Version;
        }

        public static bool operator <(FirmwareVersion a, FirmwareVersion b)
        {
            return a.Version < b.Version;
        }

        public static bool operator >(FirmwareVersion a, FirmwareVersion b)
        {
            return a.Version > b.Version;
        }
    }
}
