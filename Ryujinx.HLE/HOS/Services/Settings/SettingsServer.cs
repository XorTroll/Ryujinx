using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    class SettingsServer : ServerManager
    {
        public SettingsServer() : base("settings", 0x0100000000000009, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "set", () => new ISettingsServer() },
            { "set:fd", () => new IFirmwareDebugSettingsServer() },
            { "set:cal", () => new IFactorySettingsServer() },
            { "set:sys", () => new ISystemSettingsServer() }
        };
    }
}
