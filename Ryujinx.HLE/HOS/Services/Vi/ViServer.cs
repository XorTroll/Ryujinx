using Ryujinx.HLE.HOS.Services.Vi.Mm;
using Ryujinx.HLE.HOS.Services.Vi.Cec;
using Ryujinx.HLE.HOS.Services.Vi.Caps;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Vi
{
    class ViServer : ServerManager
    {
        public ViServer() : base("vi", 0x010000000000002D, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "vi:u", () => new IApplicationRootService() },
            { "vi:s", () => new ISystemRootService() },
            { "vi:m", () => new IManagerRootService() },
            { "mm:u", () => new IRequest() },
            { "cec-mgr", () => new ICecManager() },
            { "caps:su", () => new IScreenShotApplicationService() },
            { "caps:sc", () => new IScreenShotControlService() },
            { "caps:ss", () => new IScreenshotService() }
        };
    }
}
