using System;

namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    [Flags]
    enum UserSelectorFlag
    {
        SkipIfSingleUser = 1 << 0,
        Unknown = 1 << 31
    }
}
