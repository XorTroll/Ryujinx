using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Pctl
{
    class PctlServer : ServerManager
    {
        public PctlServer() : base("pctl", 0x010000000000002E, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "pctl", () => new IParentalControlServiceFactory(0x303) },
            { "pctl:a", () => new IParentalControlServiceFactory(0x83BE) },
            { "pctl:r", () => new IParentalControlServiceFactory(0x8040) },
            { "pctl:s", () => new IParentalControlServiceFactory(0x838E) }
        };
    }
}
