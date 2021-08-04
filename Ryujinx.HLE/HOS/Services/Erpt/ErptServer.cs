using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Erpt
{
    class ErptServer : ServerManager
    {
        public ErptServer() : base("erpt", 0x010000000000002B, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "erpt:c", () => new IContext() },
            { "erpt:r", () => new ISession() }
        };
    }
}
