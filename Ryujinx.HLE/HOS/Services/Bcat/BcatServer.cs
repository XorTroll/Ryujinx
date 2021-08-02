using Ryujinx.HLE.HOS.Services.Bcat.Prepo;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Bcat
{
    class BcatServer : ServerManager
    {
        public BcatServer(Horizon system) : base(system, "bcat", 0x010000000000000C, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "bcat:a", () => new IServiceCreator(_system, "bcat:a") },
            { "bcat:m", () => new IServiceCreator(_system, "bcat:m") },
            { "bcat:u", () => new IServiceCreator(_system, "bcat:u") },
            { "bcat:s", () => new IServiceCreator(_system, "bcat:s") },
            { "news:a", () => new News.IServiceCreator() },
            { "news:c", () => new News.IServiceCreator() },
            { "news:m", () => new News.IServiceCreator() },
            { "news:p", () => new News.IServiceCreator() },
            { "news:v", () => new News.IServiceCreator() },
            { "prepo:a", () => new IPrepoService(PrepoServicePermissionLevel.Admin) },
            { "prepo:a2", () => new IPrepoService(PrepoServicePermissionLevel.Admin) },
            { "prepo:m", () => new IPrepoService(PrepoServicePermissionLevel.Manager) },
            { "prepo:u", () => new IPrepoService(PrepoServicePermissionLevel.User) },
            { "prepo:s", () => new IPrepoService(PrepoServicePermissionLevel.System) }
        };
    }
}
