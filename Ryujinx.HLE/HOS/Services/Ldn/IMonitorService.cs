using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Ldn
{
    class IMonitorService : IpcService
    {
        [CommandHipc(0)]
        // GetStateForMonitor() -> u32
        public ResultCode GetStateForMonitor(ServiceCtx context)
        {
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.ServiceLdn);

            return ResultCode.Success;
        }

        [CommandHipc(100)]
        // InitializeMonitor()
        public ResultCode InitializeMonitor(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceLdn);

            return ResultCode.Success;
        }

        [CommandHipc(101)]
        // FinalizeMonitor()
        public ResultCode FinalizeMonitor(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceLdn);

            return ResultCode.Success;
        }
    }
}