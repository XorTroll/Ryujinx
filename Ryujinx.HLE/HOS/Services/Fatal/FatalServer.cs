using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Fatal
{
    class FatalServer : ServerManager
    {
        public FatalServer(Horizon system) : base(system, "fatal", 0x0100000000000034, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "fatal:u", () => new IService() },
            { "fatal:p", () => new IPrivateService() }
        };
    }
}
