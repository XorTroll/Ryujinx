using Ryujinx.HLE.HOS.Services.Time.Types;
using Ryujinx.HLE.HOS.SystemState;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    [StructLayout(LayoutKind.Sequential)]
    struct EulaVersion
    {
        public uint Version { get; set; }

        public uint Region { get; set; }

        public uint ClockType { get; set; }

        public uint Reserved { get; set; }

        public ulong NetworkSystemClockTime { get; set; }

        public SteadyClockContext SteadyClockTime { get; set; }

        public static EulaVersion Default = new EulaVersion
        {
            Version = 0x10000,
            Region = (uint)RegionCode.USA,
            ClockType = (uint)EulaVersionClockType.SteadyClock,
            NetworkSystemClockTime = 0,
            SteadyClockTime = new SteadyClockContext()
        };
    }
}
