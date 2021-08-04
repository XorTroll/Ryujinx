using Ryujinx.HLE.HOS.Services.Time.Types;
using Ryujinx.HLE.HOS.SystemState;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EulaVersion
    {
        public uint Version { get; set; }

        public RegionCode RegionCode { get; set; }

        public EulaVersionClockType EulaVersionClockType { get; set; }

        public uint Reserved { get; set; }

        public ulong NetworkSystemClockTime { get; set; }

        public SteadyClockContext SteadyClockTime { get; set; }

        public static EulaVersion Default = new EulaVersion
        {
            Version = 0x10000,
            RegionCode = RegionCode.USA,
            EulaVersionClockType = EulaVersionClockType.SteadyClock,
            NetworkSystemClockTime = 0,
            SteadyClockTime = new SteadyClockContext()
        };
    }
}
