using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Pcie
{
    class PcieServer : ServerManager
    {
        public PcieServer(Horizon system) : base(system, "pcie", 0x010000000000001D, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "pcie", () => new IManager() },
            { "pcie:log", () => new ILogManager() }
        };
    }
}
