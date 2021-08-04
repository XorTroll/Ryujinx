using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Nifm
{
    class NifmServer : ServerManager
    {
        public NifmServer() : base("nifm", 0x010000000000000F, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "nifm:a", () => new IStaticService() },
            { "nifm:s", () => new IStaticService() },
            { "nifm:u", () => new IStaticService() }
        };
    }
}
