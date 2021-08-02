using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Spl
{
    class SplServer : ServerManager
    {
        public SplServer(Horizon system) : base(system, "spl", 0x0100000000000028, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "spl:", () => new IGeneralInterface() },
            { "spl:es", () => new IGeneralInterface() },
            { "spl:fs", () => new IGeneralInterface() },
            { "spl:manu", () => new IGeneralInterface() },
            { "spl:mig", () => new IGeneralInterface() },
            { "spl:ssl", () => new IGeneralInterface() },
            { "csrng", () => new IRandomInterface() }
        };
    }
}
