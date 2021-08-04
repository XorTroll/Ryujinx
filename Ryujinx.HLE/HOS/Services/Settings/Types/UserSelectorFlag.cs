using System;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [Flags]
    public enum UserSelectorFlag : uint
    {
        None = 0,
        SkipIfSingleUser = 1 << 0
    }
}
