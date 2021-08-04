using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Ldn.Lp2p
{
    [Service("lp2p:m")] // ?
    class IMonitorServiceCreator : IpcService
    {
        public IMonitorServiceCreator() { }

        [CommandHipc(0)]
        // CreateMonitorService(pid, u64) -> object<nn::lp2p::detail::IMonitorService>
        public ResultCode CreateMonitorService(ServiceCtx context)
        {
            context.RequestData.ReadUInt64(); // Reserved
            var input = context.RequestData.ReadUInt64();

            Logger.Stub?.PrintStub(LogClass.ServiceLdn, new { input });

            MakeObject(context, new IMonitorService());

            return ResultCode.Success;
        }
    }
}