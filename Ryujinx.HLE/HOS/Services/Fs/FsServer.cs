using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryujinx.HLE.HOS.Services.Fs
{
    class FsServer : ServerManager
    {
        public FsServer() : base("fs", 0x0100000000000000, 44, 5) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "fsp-srv", () => new IFileSystemProxy() },
            { "fsp-ldr", () => new IFileSystemProxyForLoader() },
            { "fsp-pr", () => new IProgramRegistry() }
        };
    }
}
