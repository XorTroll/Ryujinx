using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Ldn
{
    class LdnServer : ServerManager
    {
        public LdnServer(Horizon system) : base(system, "ldn", 0x0100000000000018, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "ldn:m", () => new IMonitorServiceCreator() },
            { "ldn:s", () => new ISystemServiceCreator() },
            { "ldn:u", () => new IUserServiceCreator() },
            { "lp2p:m", () => new Lp2p.IMonitorServiceCreator() },
            { "lp2p:app", () => new Lp2p.IServiceCreator() },
            { "lp2p:sys", () => new Lp2p.IServiceCreator() }
        };
    }
}
