using Ryujinx.HLE.HOS.Services.Ns.Aoc;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Ns
{
    class NsServer : ServerManager
    {
        public NsServer(Horizon system) : base(system, "ns", 0x010000000000001F, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "ns:am", () => new IApplicationManagerInterface(_system.KernelContext) },
            { "ns:dev", () => new IDevelopInterface() },
            { "ns:am2", () => new IServiceGetterInterface() },
            { "ns:ec", () => new IServiceGetterInterface() },
            { "ns:rid", () => new IServiceGetterInterface() },
            { "ns:rt", () => new IServiceGetterInterface() },
            { "ns:web", () => new IServiceGetterInterface() },
            { "ns:ro", () => new IServiceGetterInterface() },
            { "ns:vm", () => new IVulnerabilityManagerInterface() },
            { "ns:su", () => new ISystemUpdateInterface(_system.KernelContext) },
            { "aoc:u", () => new IAddOnContentManager(_system.KernelContext) }
        };
    }
}
