using Ryujinx.HLE.HOS.Services.Pcv.Bpc;
using Ryujinx.HLE.HOS.Services.Pcv.Clkrst;
using Ryujinx.HLE.HOS.Services.Pcv.Rgltr;
using Ryujinx.HLE.HOS.Services.Pcv.Rtc;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Pcv
{
    class PcvServer : ServerManager
    {
        public PcvServer() : base("pcv", 0x010000000000001A, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "pcv", () => new IPcvService() },
            { "bpc", () => new IBoardPowerControlManager() },
            { "bpc:r", () => new IRtcManager() },
            { "clkrst", () => new IClkrstManager() },
            { "clkrst:i", () => new IClkrstManager() },
            { "clkrst:a", () => new IArbitrationManager() },
            { "rgltr", () => new IRegulatorManager() },
            { "rtc", () => new IUnknown1() }
        };
    }
}
