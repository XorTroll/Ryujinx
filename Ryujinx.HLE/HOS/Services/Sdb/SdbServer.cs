using Ryujinx.HLE.HOS.Services.Sdb.Pl;
using Ryujinx.HLE.HOS.Services.Sdb.Mii;
using Ryujinx.HLE.HOS.Services.Sdb.Pdm;
using Ryujinx.HLE.HOS.Services.Sdb.Avm;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Sdb
{
    class SdbServer : ServerManager
    {
        public SdbServer() : base("sdb", 0x0100000000000039, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "pl:u", () => new ISharedFontManager() },
            { "pl:s", () => new ISharedFontManager() },
            { "mii:e", () => new IStaticService(true) },
            { "mii:u", () => new IStaticService(false) },
            { "miiimg", () => new IImageDatabaseService() },
            { "pdm:ntfy", () => new INotifyService() },
            { "pdm:qry", () => new IQueryService() },
            { "avm", () => new IAvmService() }
        };
    }
}
