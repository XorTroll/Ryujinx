using System;

namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    [Flags]
    enum AccountNotificationFlag
    {
        FriendOnlineFlag = 1 << 0,
        FriendRequestFlag = 1 << 1,
        CoralInvitationFlag = 1 << 8,
    }
}
