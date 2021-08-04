using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    [StructLayout(LayoutKind.Sequential)]
    struct SleepSettings
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
