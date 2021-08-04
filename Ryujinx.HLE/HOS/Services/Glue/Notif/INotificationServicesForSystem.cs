using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Threading;

namespace Ryujinx.HLE.HOS.Services.Glue.Notif
{
    [Service("notif:s")] // 9.0.0+
    class INotificationServicesForSystem : IpcService
    {
        private KEvent _unkEvent;

        public INotificationServicesForSystem()
        {
            _unkEvent = new KEvent(Horizon.Instance.KernelContext);
        }

        [CommandHipc(520)]
        // ListAlarmSettings() -> buffer, u32
        public ResultCode ListAlarmSettings(ServiceCtx context)
        {
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }

        [CommandHipc(1040)]
        // OpenUnkEventAccessor() -> object<nn::notification::server::INotificationSystemEventAccessor>
        public ResultCode OpenUnkEventAccessor(ServiceCtx context)
        {
            MakeObject(context, new INotificationSystemEventAccessor(_unkEvent));

            return ResultCode.Success;
        }

        [CommandHipc(1510)]
        // GetPresentationSetting() -> ?
        public ResultCode GetPresentationSetting(ServiceCtx context)
        {
            // TODO

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }
    }
}