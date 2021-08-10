using Ryujinx.Common;
using Ryujinx.Common.Logging;

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

        [CommandHipc(21)]
        // PushContext(object<nn::am::service::IStorage>)
        public ResultCode PushContext(ServiceCtx context)
        {
            var data = GetObject<IStorage>(context, 0);

            _self.PushContext(data.Data);

            return ResultCode.Success;
        }

        [CommandHipc(22)]
        // PopContext() -> object<nn::am::service::IStorage>
        public ResultCode PopContext(ServiceCtx context)
        {
            if(_self.TryPopContext(out var data))
            {
                MakeObject(context, new IStorage(data));

                return ResultCode.Success;
            }
            else
            {
                Logger.Warning?.Print(LogClass.ServiceAm, "No storages available!");
                return ResultCode.NotAvailable;
            }
        }
    }
}