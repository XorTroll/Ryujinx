using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.SystemAppletProxy
{
    class IApplicationCreator : IpcService
    {
        public IApplicationCreator() { }

        [CommandHipc(10)]
        // CreateSystemApplication()
        public ResultCode CreateSystemApplication(ServiceCtx context)
        {
            var appId = context.RequestData.ReadUInt64();

            Logger.Info?.Print(LogClass.ServiceAm, "Creating system application 0x" + appId.ToString("X16"));

            throw new System.Exception("Hey, stop here");

            return ResultCode.Success;
        }
    }
}