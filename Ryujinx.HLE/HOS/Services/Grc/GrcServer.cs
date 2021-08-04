using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Grc
{
    class GrcServer : ServerManager
    {
        public GrcServer() : base("grc", 0x0100000000000035, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "grc:c", () => new IGrcService() },
            { "grc:d", () => new IRemoteVideoTransfer() }
        };
    }
}
