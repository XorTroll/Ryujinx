using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy
{
    class IAppletCommonFunctions : IpcService
    {
        public IAppletCommonFunctions() { }

        [CommandHipc(42)]
        // SetDisplayMagnification(float, float, float, float)
        public ResultCode SetDisplayMagnification(ServiceCtx context)
        {
            var x = context.RequestData.ReadSingle();
            var y = context.RequestData.ReadSingle();
            var width = context.RequestData.ReadSingle();
            var height = context.RequestData.ReadSingle();

            Logger.Stub?.PrintStub(LogClass.ServiceAm, new { x, y, width, height });

            return ResultCode.Success;
        }
    }
}