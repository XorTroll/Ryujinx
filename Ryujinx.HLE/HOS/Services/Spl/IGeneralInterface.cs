namespace Ryujinx.HLE.HOS.Services.Spl
{
    [Service("spl:")]
    [Service("spl:es")]
    [Service("spl:fs")]
    [Service("spl:manu")]
    [Service("spl:mig")]
    [Service("spl:ssl")] 
    class IGeneralInterface : IpcService
    {
    }
}