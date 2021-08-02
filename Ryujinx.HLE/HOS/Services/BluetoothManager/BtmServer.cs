using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.BluetoothManager
{
    class BtmServer : ServerManager
    {
        public BtmServer(Horizon system) : base(system, "btm", 0x010000000000002A, 44, 1) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "btm", () => new IBtm() },
            { "btm:dbg", () => new IBtmDebug() },
            { "btm:sys", () => new IBtmSystem() },
            { "btm:u", () => new IBtmUser() }
        };
    }
}
