using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SleepSettings
    {
        public uint Flags { get; set; }

        public uint HandheldSleep { get; set; }

        public uint ConsoleSleep { get; set; }

        public static SleepSettings Default = new()
        {
            Flags = 0,
            HandheldSleep = (uint)HandheldSleepPlan.Never,
            ConsoleSleep = (uint)ConsoleSleepPlan.Never
        };
    }
}
