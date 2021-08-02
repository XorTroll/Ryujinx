using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Nv.NvDrvServices.NvHostAsGpu.Types
{
    [StructLayout(LayoutKind.Sequential)]
    struct AllocAsExArguments
    {
        public uint BigPageSize;
        public int   AsFd;
        public uint  Flags;
        public uint  Reserved;
        public ulong Unknown0;
        public ulong Unknown1;
        public ulong Unknown2;
    }
}
