using System;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [Flags]
    public enum NotificationFlag : uint
    {
        None = 0,
        RingtoneFlag = 1 << 0,
        DownloadCompletionFlag = 1 << 1,
        EnablesNews = 1 << 8,
        IncomingLampFlag = 1 << 9
    }
}
