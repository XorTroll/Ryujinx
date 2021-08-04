using Ryujinx.HLE.HOS.Services.Psc.Srepo;
using Ryujinx.HLE.HOS.Services.Psc.Ovln;
using Ryujinx.HLE.HOS.Services.Psc.Time;
using Ryujinx.HLE.HOS.Services.Psc.Ins;
using Ryujinx.HLE.HOS.Services.Time;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Psc
{
    class PscServer : ServerManager
    {
        public PscServer() : base("psc", 0x0100000000000021, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "psc:c", () => new IPmControl() },
            { "psc:m", () => new IPmService() },
            { "psc:l", () => new IPmUnknown() },
            { "srepo:a", () => new ISrepoService() },
            { "srepo:u", () => new ISrepoService() },
            { "ovln:rcv", () => new IReceiverService() },
            { "ovln:snd", () => new ISenderService() },
            { "time:m", () => new ITimeServiceManager() },
            { "time:s", () => new IStaticServiceForPsc(TimeManager.Instance, TimePermissions.System) },
            { "time:su", () => new IStaticServiceForPsc(TimeManager.Instance, TimePermissions.SystemUpdate) },
            { "time:al", () => new IAlarmService() },
            { "time:p", () => new IPowerStateRequestHandler() },
            { "ins:r", () => new IReceiverManager() },
            { "ins:s", () => new ISenderManager() }
        };
    }
}
