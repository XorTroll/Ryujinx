using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Es
{
    class EsServer : ServerManager
    {
        public EsServer(Horizon system) : base(system, "es", 0x0100000000000033, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "es", () => new IETicketService() }
        };
    }
}
