using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Loader
{
    class LoaderServer : ServerManager
    {
        public LoaderServer(Horizon system) : base(system, "loader", 0x0100000000000001, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "ldr:shel", () => new IShellInterface() },
            { "ldr:dmnt", () => new IDebugMonitorInterface() },
            { "ldr:pm", () => new IProcessManagerInterface() }
        };
    }
}
