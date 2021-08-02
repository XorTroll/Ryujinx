using System;

namespace Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Npad
{
    [Flags]
    enum NpadSystemButtonProperties : uint
    {
        None = 0,
        IsUnintendedHomeButtonInputProtectionEnabled = 1 << 0
    }
}