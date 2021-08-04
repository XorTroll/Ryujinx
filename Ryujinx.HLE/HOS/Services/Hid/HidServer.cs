using Ryujinx.HLE.HOS.Services.Hid.Irs;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Hid
{
    class HidServer : ServerManager
    {
        public HidServer() : base("hid", 0x0100000000000013, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "hid", () => new IHidServer() },
            { "hid:sys", () => new IHidSystemServer() },
            { "hid:dbg", () => new IHidDebugServer() },
            { "hidbus", () => new IHidbusServer() },
            { "irs", () => new IIrSensorServer() },
            { "irs:sys", () => new IIrSensorSystemServer() },
            { "xcd:sys", () => new ISystemServer() }
        };
    }
}
