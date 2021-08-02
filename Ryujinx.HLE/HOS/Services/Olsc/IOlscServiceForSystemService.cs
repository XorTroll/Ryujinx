using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;

namespace Ryujinx.HLE.HOS.Services.Olsc
{
    [Service("olsc:s")] // 4.0.0+
    class IOlscServiceForSystemService : IpcService
    {
        public IOlscServiceForSystemService() { }

        [CommandHipc(0)]
        // GetTransferTaskListController() -> object<nn::olsc::srv::ITransferTaskListController>
        public ResultCode GetTransferTaskListController(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceOlsc);

            MakeObject(context, new ITransferTaskListController(context));

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(0xBABE);

            return ResultCode.Success;
        }
    }
}