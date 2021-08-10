using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    struct AppletIdentifyInfo
    {
        public AppletId AppletId;
        public uint     Padding;
        public ulong    ApplicationId;
    }
}