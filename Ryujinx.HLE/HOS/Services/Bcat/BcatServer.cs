using Ryujinx.HLE.HOS.Services.Bcat.Prepo;
using System;
using System.Collections.Generic;
using Ryujinx.HLE.HOS.Services.Settings;

namespace Ryujinx.HLE.HOS.Services.Bcat
{
    class BcatServer : ServerManager
    {
        public BcatServer() : base("bcat", 0x010000000000000C, 44) { }

        public static IpcService NewsServiceFactory()
        {
            if (Horizon.Instance.ContentManager.FirmwareVersion > new FirmwareVersion(1, 0, 0))
            {
                return new News.IServiceCreator();
            }
            else
            {
                return new News.INewsService();
            }
        }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "bcat:a", () => new IServiceCreator("bcat:a") },
            { "bcat:m", () => new IServiceCreator("bcat:m") },
            { "bcat:u", () => new IServiceCreator("bcat:u") },
            { "bcat:s", () => new IServiceCreator("bcat:s") },
            { "news:a", NewsServiceFactory },
            { "news:c", NewsServiceFactory },
            { "news:m", NewsServiceFactory },
            { "news:p", NewsServiceFactory },
            { "news:v", NewsServiceFactory },
            { "prepo:a", () => new IPrepoService(PrepoServicePermissionLevel.Admin) },
            { "prepo:a2", () => new IPrepoService(PrepoServicePermissionLevel.Admin) },
            { "prepo:m", () => new IPrepoService(PrepoServicePermissionLevel.Manager) },
            { "prepo:u", () => new IPrepoService(PrepoServicePermissionLevel.User) },
            { "prepo:s", () => new IPrepoService(PrepoServicePermissionLevel.System) }
        };
    }
}
