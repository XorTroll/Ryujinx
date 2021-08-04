using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Erpt
{
    [Service("erpt:c")]
    class IContext : IpcService
    {
        [CommandHipc(0)]
        // SubmitContext(buffer, buffer)
        public ResultCode SubmitContext(ServiceCtx context)
        {
            // TODO: error report context format parsing, etc.

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }
    }
}