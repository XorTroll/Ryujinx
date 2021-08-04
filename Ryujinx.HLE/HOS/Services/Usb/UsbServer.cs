using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Usb
{
    class UsbServer : ServerManager
    {
        public UsbServer() : base("usb", 0x0100000000000006, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "usb:ds", () => new IDsService() },
            { "usb:hs", () => new IClientRootSession() },
            { "usb:hs:a", () => new IClientRootSession() },
            { "usb:pd", () => new IPdManager() },
            { "usb:pd:c", () => new IPdCradleManager() },
            { "usb:pd:m", () => new IPdManufactureManager() },
            { "usb:pm", () => new IPmService() },
            { "usb:qdb", () => new IUnknown1() },
            { "usb:obsv", () => new IUnknown2() }
        };
    }
}
