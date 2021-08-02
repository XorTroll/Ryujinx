using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Ro
{
    class RoServer : ServerManager
    {
        public RoServer(Horizon system) : base(system, "ro", 0x0100000000000037, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "ldr:ro", () => new IRoInterface() },
            { "ro:1", () => new IRoInterface() }
        };
    }
}
