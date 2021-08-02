namespace Ryujinx.HLE.HOS.Services.Ptm.Fgm
{
    [Service("fgm")]   // 2.0.0+
    [Service("fgm:0")] // 2.0.0+
    [Service("fgm:9")] // 2.0.0+
    class ISession : IpcService
    {
        public ISession() { }
    }
}