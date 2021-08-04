using Ryujinx.HLE.HOS.Services.Glue.Arp;
using Ryujinx.HLE.HOS.Services.Glue.Bgct;
using Ryujinx.HLE.HOS.Services.Glue.Ectx;
using Ryujinx.HLE.HOS.Services.Glue.Notif;
using Ryujinx.HLE.HOS.Services.Glue.Time;
using Ryujinx.HLE.HOS.Services.Time;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Glue
{
    class GlueServer : ServerManager
    {
        public GlueServer() : base("glue", 0x0100000000000031, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "arp:r", () => new IReader() },
            { "arp:w", () => new IWriter() },
            { "bgtc:sc", () => new IStateControlService() },
            { "bgtc:t", () => new ITaskService() },
            { "ectx:r", () => new IReaderForSystem() },
            { "ectx:aw", () => new IWriterForApplication() },
            { "ectx:w", () => new IWriterForSystem() },
            { "notif:a", () => new INotificationServicesForApplication() },
            { "notif:s", () => new INotificationServicesForSystem() },
            { "time:a", () => new IStaticServiceForGlue(TimePermissions.Admin) },
            { "time:r", () => new IStaticServiceForGlue(TimePermissions.Repair) },
            { "time:u", () => new IStaticServiceForGlue(TimePermissions.User) }
        };
    }
}
