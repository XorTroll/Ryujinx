using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryujinx.HLE.HOS.Kernel.Process
{
    [Flags]
    public enum BreakReason : uint
    {
        Panic = 0,
        Assert = 1,
        User = 2,
        PreLoadDll = 3,
        PostLoadDll = 4,
        PreUnloadDll = 5,
        PostUnloadDll = 6,
        CppException = 7,
        NotificationOnlyFlag = 0x80000000
    }
}
