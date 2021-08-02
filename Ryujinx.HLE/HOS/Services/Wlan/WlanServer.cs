using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Wlan
{
    class WlanServer : ServerManager
    {
        public WlanServer(Horizon system) : base(system, "wlan", 0x0100000000000016, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "wlan:inf", () => new IInfraManager() },
            { "wlan:lga", () => new ILocalGetActionFrame() },
            { "wlan:lg", () => new ILocalGetFrame() },
            { "wlan:lcl", () => new ILocalManager() },
            { "wlan:sg", () => new ISocketGetFrame() },
            { "wlan:soc", () => new ISocketManager() },
            { "wlan:dtc", () => new IUnknown1() }
        };
    }
}
