using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Caps
{
    class CapsServer : ServerManager
    {
        public CapsServer() : base("capsrv", 0x0100000000000022, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "caps:a", () => new IAlbumAccessorService() },
            { "caps:u", () => new IAlbumApplicationService() },
            { "caps:c", () => new IAlbumControlService() }
        };
    }
}
