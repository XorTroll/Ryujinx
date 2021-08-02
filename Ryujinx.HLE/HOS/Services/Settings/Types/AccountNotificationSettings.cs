using Ryujinx.HLE.HOS.Services.Account.Acc;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    [StructLayout(LayoutKind.Sequential)]
    struct AccountNotificationSettings
    {
        public UserId UserId { get; set; }

        public uint Flags { get; set; }

        public byte PresenceOverlayPermission { get; set; }

        public byte InvitationOverlayPermission { get; set; }

        public ushort Reserved { get; set; }

        public static AccountNotificationSettings MakeDefault(UserId user_id) => new AccountNotificationSettings
        {
            UserId = user_id,
            Flags = 0,
            PresenceOverlayPermission = (byte)FriendPresenceOverlayPermission.Friends,
            InvitationOverlayPermission = (byte)FriendPresenceOverlayPermission.Friends
        };
    }
}
