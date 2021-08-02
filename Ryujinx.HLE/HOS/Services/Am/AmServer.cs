using Ryujinx.HLE.HOS.Services.Am.AppletAE;
using Ryujinx.HLE.HOS.Services.Am.AppletOE;
using Ryujinx.HLE.HOS.Services.Am.Idle;
using Ryujinx.HLE.HOS.Services.Am.Omm;
using Ryujinx.HLE.HOS.Services.Am.Spsm;
using Ryujinx.HLE.HOS.Services.Am.Tcap;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Am
{
    class AmServer : ServerManager
    {
        public AmServer(Horizon system) : base(system, "am", 0x0100000000000023, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "appletAE", () => new IAllSystemAppletProxiesService() },
            { "appletOE", () => new IApplicationProxyService() },
            { "idle:sys", () => new IPolicyManagerSystem() },
            { "omm", () => new IOperationModeManager() },
            { "spsm", () => new IPowerStateInterface() },
            { "tcap", () => new IManager() }
        };
    }
}
