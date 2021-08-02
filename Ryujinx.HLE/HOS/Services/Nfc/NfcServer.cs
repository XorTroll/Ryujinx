using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Nfc
{
    class NfcServer : ServerManager
    {
        public NfcServer(Horizon system) : base(system, "nfc", 0x0100000000000020, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "nfc:am", () => new IAmManager() },
            { "nfc:mf:u", () => new Mifare.IUserManager() },
            { "nfc:sys", () => new ISystemManager() },
            { "nfc:user", () => new IUserManager() },
            { "nfp:sys", () => new Nfp.ISystemManager() },
            { "nfp:user", () => new Nfp.IUserManager() },
            { "nfp:dbg", () => new Nfp.IDebugManager() }
        };
    }
}
