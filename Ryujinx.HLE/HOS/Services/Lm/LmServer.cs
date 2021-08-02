using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Lm
{
    class LmServer : ServerManager
    {
        public LmServer(Horizon system) : base(system, "LogManager", 0x0100000000000015, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "lm", () => new ILogService() },
            { "lm:get", () => new ILogGetter() }
        };
    }
}
