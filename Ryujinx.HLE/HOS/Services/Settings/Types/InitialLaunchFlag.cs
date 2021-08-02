using System;

namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    [Flags]
    enum InitialLaunchFlag
    {
        CompletionFlag = 1 << 0,
        UserAdditionFlag = 1 << 8,
        TimestampFlag = 1 << 16
    }
}
