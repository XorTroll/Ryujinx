using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Mig
{
    class MigServer : ServerManager
    {
        public MigServer() : base("migration", 0x010000000000003A, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "mig:usr", () => new IService() }
        };
    }
}
