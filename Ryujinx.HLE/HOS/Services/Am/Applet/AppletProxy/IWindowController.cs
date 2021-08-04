using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy
{
    class IWindowController : IpcService
    {
        private readonly long _pid;

        public IWindowController(long pid)
        {
            _pid = pid;
        }

        [CommandHipc(1)]
        // GetAppletResourceUserId() -> nn::applet::AppletResourceUserId
        public ResultCode GetAppletResourceUserId(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            long appletResourceUserId = Horizon.Instance.AppletState.AppletResourceUserIds.Add(_pid);

            context.ResponseData.Write(appletResourceUserId);

            return ResultCode.Success;
        }

        [CommandHipc(10)]
        // AcquireForegroundRights()
        public ResultCode AcquireForegroundRights(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }
    }
}