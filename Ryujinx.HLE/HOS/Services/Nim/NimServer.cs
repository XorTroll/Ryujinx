using Ryujinx.HLE.HOS.Services.Nim.Ntc;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Nim
{
    class NimServer : ServerManager
    {
        public NimServer(Horizon system) : base(system, "nim", 0x0100000000000025, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "nim", () => new INetworkInstallManager() },
            { "nim:shp", () => new IShopServiceManager() },
            { "nim:eca", () => new IShopServiceAccessServerInterface() },
            { "nim:ecas", () => new IShopServiceAccessSystemInterface() },
            { "ntc", () => new IStaticService() }
        };
    }
}
