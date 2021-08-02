using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Bluetooth
{
    class BluetoothServer : ServerManager
    {
        public BluetoothServer(Horizon system) : base(system, "bluetooth", 0x010000000000000B, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "btdrv", () => new IBluetoothDriver() },
            { "bt", () => new IBluetoothUser() }
        };
    }
}
