using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Npns
{
    class NpnsServer : ServerManager
    {
        public NpnsServer(Horizon system) : base(system, "npns", 0x010000000000002F, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "npns:u", () => new INpnsUser(_system.KernelContext) },
            { "npns:s", () => new INpnsSystem() }
        };
    }
}
