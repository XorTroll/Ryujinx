using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Ssl
{
    class SslServer : ServerManager
    {
        public SslServer(Horizon system) : base(system, "ssl", 0x0100000000000024, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "ssl", () => new ISslService() }
        };
    }
}
