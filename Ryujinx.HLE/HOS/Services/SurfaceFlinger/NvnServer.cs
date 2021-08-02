using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.SurfaceFlinger
{
    class NvnServer : ServerManager
    {
        public NvnServer(Horizon system) : base(system, "nvnflinger", 0x010000000000001C, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "dispdrv", () => new HOSBinderDriverServer() }
        };
    }
}
