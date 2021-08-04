using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Ptm
{
    class PtmServer : ServerManager
    {
        public PtmServer() : base("ptm", 0x0100000000000010, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "fan", () => new Fan.IManager() },
            { "fgm", () => new Fgm.ISession() },
            { "fgm:0", () => new Fgm.ISession() },
            { "fgm:9", () => new Fgm.ISession() },
            { "fgm:dbg", () => new Fgm.IDebugger() },
            { "pcm", () => new Pcm.IManager() },
            { "psm", () => new Psm.IPsmServer() },
            { "tc", () => new Tc.IManager() },
            { "ts", () => new Ts.IMeasurementServer() },
            { "lbl", () => new Lbl.LblControllerServer() },
            { "apm", () => new Apm.ManagerServer() },
            { "apm:am", () => new Apm.ManagerServer() },
            { "apm:sys", () => new Apm.SystemManagerServer() }
        };
    }
}
