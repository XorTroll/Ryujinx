using Ryujinx.HLE.HOS.Services.Sockets.Bsd;
using Ryujinx.HLE.HOS.Services.Sockets.Ethc;
using Ryujinx.HLE.HOS.Services.Sockets.Nsd;
using Ryujinx.HLE.HOS.Services.Sockets.Sfdnsres;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Sockets
{
    class SocketsServer : ServerManager
    {
        public SocketsServer() : base("bsdsockets", 0x0100000000000012, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "bsd:s", () => new IClient(true) },
            { "bsd:u", () => new IClient(false) },
            { "bsdcfg", () => new ServerInterface() },
            { "ethc:c", () => new IEthInterface() },
            { "ethc:i", () => new IEthInterfaceGroup() },
            { "sfdnsres", () => new IResolver() },
            { "nsd:a", () => new IManager() },
            { "nsd:u", () => new IManager() }
        };
    }
}
