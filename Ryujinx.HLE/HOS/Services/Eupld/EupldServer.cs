using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Eupld
{
    class EupldServer : ServerManager
    {
        public EupldServer(Horizon system) : base(system, "eupld", 0x0100000000000030, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "eupld:r", () => new IRequest() },
            { "eupld:c", () => new IControl() }
        };
    }
}
