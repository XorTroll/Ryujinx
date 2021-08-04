using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Npns
{
    class NpnsServer : ServerManager
    {
        public NpnsServer() : base("npns", 0x010000000000002F, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "npns:u", () => new INpnsUser() },
            { "npns:s", () => new INpnsSystem() }
        };
    }
}
