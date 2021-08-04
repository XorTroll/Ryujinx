using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Pm
{
    class PmServer : ServerManager
    {
        public PmServer() : base("ProcessMana", 0x0100000000000003, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "pm:bm", () => new IBootModeInterface() },
            { "pm:dmnt", () => new IDebugMonitorInterface() },
            { "pm:info", () => new IInformationInterface() },
            { "pm:shell", () => new IShellInterface() }
        };
    }
}
