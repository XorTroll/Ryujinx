using Ryujinx.Common;

namespace Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy
{
    class IProcessWindingController : IpcService
    {
        private AppletContext _self;

        public IProcessWindingController(AppletContext self)
        {
            _self = self;
        }

        [CommandHipc(0)]
        // GetLaunchReason() -> nn::am::service::AppletProcessLaunchReason
        public ResultCode GetLaunchReason(ServiceCtx context)
        {
            // NOTE: Flag is set by using an internal field.

            context.ResponseData.WriteStruct(_self.LaunchReason);

            return ResultCode.Success;
        }
    }
}