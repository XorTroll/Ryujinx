using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Services.Glue.Arp;

namespace Ryujinx.HLE.HOS.Services.Bcat.ServiceCreator
{
    class IBcatService : IpcService
    {
        public IBcatService(ApplicationLaunchProperty applicationLaunchProperty) { }

        [CommandHipc(10100)]
        // RequestSyncDeliveryCache() -> object<nn::bcat::detail::ipc::IDeliveryCacheProgressService>
        public ResultCode RequestSyncDeliveryCache(ServiceCtx context)
        {
            MakeObject(context, new IDeliveryCacheProgressService(context));

            return ResultCode.Success;
        }

        [CommandHipc(30300)]
        // RegisterSystemApplicationDeliveryTasks(?) -> ?
        public ResultCode RegisterSystemApplicationDeliveryTasks(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceBcat);

            return ResultCode.Success;
        }
    }
}