using System;

namespace Ryujinx.HLE.HOS.Services.Hid.SharedMemory.DebugPad
{
    [Flags]
    enum DebugPadAttribute : uint
    {
        None = 0,
        Connected = 1 << 0
    }
}