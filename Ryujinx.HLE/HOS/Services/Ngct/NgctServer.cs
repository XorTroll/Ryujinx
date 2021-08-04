using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Ngct
{
    class NgctServer : ServerManager
    {
        public NgctServer() : base("ngct", 0x0100000000000041, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "ngct:u", () => new IService() },
            { "ngct:s", () => new IServiceWithManagementApi() }
        };
    }
}
