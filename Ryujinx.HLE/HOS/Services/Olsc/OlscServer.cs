using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Olsc
{
    class OlscServer : ServerManager
    {
        public OlscServer() : base("olsc", 0x010000000000003E, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "olsc:s", () => new IOlscServiceForSystemService() },
            { "olsc:u", () => new IOlscServiceForApplication() }
        };
    }
}
