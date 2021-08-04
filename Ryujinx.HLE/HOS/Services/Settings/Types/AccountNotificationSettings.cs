using Ryujinx.HLE.HOS.Services.Account.Acc;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AccountNotificationSettings
    {
        public UserId UserId { get; set; }

        public AccountNotificationFlag AccountNotificationFlags { get; set; }

        public FriendPresenceOverlayPermission FriendPresenceOverlayPermission { get; set; }

        public FriendPresenceOverlayPermission FriendInvitationOverlayPermission { get; set; }

        public ushort Reserved { get; set; }

        public static AccountNotificationSettings MakeDefault(UserId user_id) => new AccountNotificationSettings
        {
            UserId = user_id,
            AccountNotificationFlags = AccountNotificationFlag.None,
            FriendPresenceOverlayPermission = FriendPresenceOverlayPermission.Friends,
            FriendInvitationOverlayPermission = FriendPresenceOverlayPermission.Friends
        };
    }
}
