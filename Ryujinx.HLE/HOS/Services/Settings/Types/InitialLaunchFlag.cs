using System;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [Flags]
    public enum InitialLaunchFlag : uint
    {
        None = 0,
        CompletionFlag = 1 << 0,
        UserAdditionFlag = 1 << 8,
        TimestampFlag = 1 << 16
    }
}
