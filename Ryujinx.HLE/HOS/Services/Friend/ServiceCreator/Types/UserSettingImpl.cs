using Ryujinx.Common.Memory;
using Ryujinx.HLE.HOS.Services.Account.Acc;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Friend.ServiceCreator
{
    [StructLayout(LayoutKind.Sequential, Size = 0x800)]
    public struct UserSettingImpl
    {
        public UserId UserId { get; set; }

        public uint PresencePermission { get; set; }

        public uint PlayLogPermission { get; set; }

        public ulong FriendRequestReception { get; set; }

        public Array32<byte> FriendCode { get; set; }

        public ulong FriendCodeNextIssuableTime { get; set; }

        // TODO: extra 0x7C8 ^^
    }
}
