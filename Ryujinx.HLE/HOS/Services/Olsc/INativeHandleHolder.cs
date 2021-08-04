using Ryujinx.HLE.HOS.Ipc;

namespace Ryujinx.HLE.HOS.Services.Olsc
{
    class INativeHandleHolder : IpcService
    {
        private int _handle;

        public INativeHandleHolder(int handle)
        {
            _handle = handle;
        }

        [CommandHipc(0)]
        // GetNativeHandle() -> handle<copy>?
        public ResultCode GetNativeHandle(ServiceCtx context)
        {
            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_handle);

            return ResultCode.Success;
        }
    }
}