using System;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [Flags]
    public enum TvFlag
    {
        Allows4k = 1 << 0,
        Allows3d = 1 << 1,
        AllowsCec = 1 << 2,
        PreventsScreenBurnIn = 1 << 3
    }
}
