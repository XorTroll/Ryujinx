using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Olsc
{
    class ITransferTaskListController : IpcService
    {
        public ITransferTaskListController(ServiceCtx context) { }

        [CommandHipc(5)]
        // GetUnkHandleHolder() -> object<nn::olsc::srv::INativeHandleHolder>
        public ResultCode GetUnkHandleHolder(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceOlsc);

            int handle = 0xBEEF1; // ???
            MakeObject(context, new INativeHandleHolder(handle));

            return ResultCode.Success;
        }

        [CommandHipc(9)]
        // GetUnk2HandleHolder() -> object<nn::olsc::srv::INativeHandleHolder>
        public ResultCode GetUnk2HandleHolder(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceOlsc);

            int handle = 0xBEEF2; // ???
            MakeObject(context, new INativeHandleHolder(handle));

            return ResultCode.Success;
        }
    }
}