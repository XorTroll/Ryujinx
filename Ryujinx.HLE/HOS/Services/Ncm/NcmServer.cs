using Ryujinx.HLE.HOS.Services.Ncm.Lr;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Ncm
{
    class NcmServer : ServerManager
    {
        public NcmServer(Horizon system) : base(system, "ncm", 0x0100000000000002, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "ncm", () => new IContentManager() },
            { "lr", () => new ILocationResolverManager() }
        };
    }
}
