using Ryujinx.Common.Memory;

namespace Ryujinx.HLE.HOS.Services.Nifm.StaticService
{
    public struct NetworkInterfaceInfo
    {
        public byte Unk1 { get; set; }

        public Array6<byte> MACAddress { get; set; }

        public byte Unk2 { get; set; }
    }
}
