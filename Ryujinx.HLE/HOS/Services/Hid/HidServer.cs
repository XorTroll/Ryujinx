using Ryujinx.HLE.HOS.Services.Hid.Irs;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Hid
{
    class HidServer : ServerManager
    {
        public HidServer(Horizon system) : base(system, "hid", 0x0100000000000013, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "hid", () => new IHidServer(_system.KernelContext) },
            { "hid:sys", () => new IHidSystemServer(_system.KernelContext) },
            { "hid:dbg", () => new IHidDebugServer() },
            { "hidbus", () => new IHidbusServer() },
            { "irs", () => new IIrSensorServer() },
            { "irs:sys", () => new IIrSensorSystemServer() }
        };
    }
}
