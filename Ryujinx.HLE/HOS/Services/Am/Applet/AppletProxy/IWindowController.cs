using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy
{
    class IWindowController : IpcService
    {
        private AppletContext _self;

        public IWindowController(AppletContext self)
        {
            _self = self;
        }

        [CommandHipc(1)]
        // GetAppletResourceUserId() -> nn::applet::AppletResourceUserId
        public ResultCode GetAppletResourceUserId(ServiceCtx context)
        {
            context.ResponseData.Write(_self.AppletResourceUserId);

            return ResultCode.Success;
        }

        [CommandHipc(10)]
        // AcquireForegroundRights()
        public ResultCode AcquireForegroundRights(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(11)]
        // ReleaseForegroundRights()
        public ResultCode ReleaseForegroundRights(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }
    }
}