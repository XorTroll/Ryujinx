using System;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [Flags]
    public enum AccountNotificationFlag : uint
    {
        None = 0,
        FriendOnlineFlag = 1 << 0,
        FriendRequestFlag = 1 << 1,
        CoralInvitationFlag = 1 << 8,
    }
}
