using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Nv
{
    class NvServer : ServerManager
    {
        public NvServer(Horizon system) : base(system, "nvservices", 0x0100000000000019, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "nvdrv", () => new INvDrvServices() },
            { "nvdrv:a", () => new INvDrvServices() },
            { "nvdrv:s", () => new INvDrvServices() },
            { "nvdrv:t", () => new INvDrvServices() },
            { "nvmemp", () => new IMemoryProfiler() },
            { "nvdrvdbg", () => new INvDrvDebugFSServices() },
            { "nvgem:c", () => new INvGemControl() },
            { "nvgem:cd", () => new INvGemCoreDump() },
            { "nvdbg:d", () => new INvDrvDebugSvcServices() }
        };
    }
}
