using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Ns
{
    class IDownloadTaskInterface : IpcService
    {
        public IDownloadTaskInterface(ServiceCtx context) { }

        [CommandHipc(707)]
        // EnableAutoCommit()
        public ResultCode EnableAutoCommit(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceNs);

            return ResultCode.Success;
        }
    }
}