using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Friend
{
    class FriendsServer : ServerManager
    {
        public FriendsServer() : base("friends", 0x010000000000000E, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "friend:a", () => new IServiceCreator(ServiceCreator.FriendServicePermissionLevel.Administrator) },
            { "friend:m", () => new IServiceCreator(ServiceCreator.FriendServicePermissionLevel.Manager) },
            { "friend:s", () => new IServiceCreator(ServiceCreator.FriendServicePermissionLevel.System) },
            { "friend:u", () => new IServiceCreator(ServiceCreator.FriendServicePermissionLevel.User) },
            { "friend:v", () => new IServiceCreator(ServiceCreator.FriendServicePermissionLevel.Viewer) }
        };
    }
}
