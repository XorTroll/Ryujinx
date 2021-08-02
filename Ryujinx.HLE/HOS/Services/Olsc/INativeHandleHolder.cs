using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;

namespace Ryujinx.HLE.HOS.Services.Olsc
{
    class INativeHandleHolder : IpcService
    {
        private int _handle;

        public INativeHandleHolder(ServiceCtx context, int handle)
        {
            _handle = handle;
        }

        [CommandHipc(0)]
        // GetNativeHandle() -> handle
        public ResultCode GetTransferTaskListController(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceOlsc);

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_handle);

            return ResultCode.Success;
        }
    }
}