using Ryujinx.HLE.Utilities;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Time.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SteadyClockContext
    {
        public ulong   InternalOffset;
        public UInt128 ClockSourceId;
    }
}
