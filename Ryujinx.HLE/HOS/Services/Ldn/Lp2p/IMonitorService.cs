using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Ldn.Lp2p
{
    class IMonitorService : IpcService
    {
        private GroupInfo _groupInfo;

        public IMonitorService()
        {
            // TODO: initial values here?
            _groupInfo = new GroupInfo();
        }

        [CommandHipc(0)]
        // Initialize()
        public ResultCode Initialize(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceLdn);

            return ResultCode.Success;
        }

        [CommandHipc(288)]
        // GetGroupInfo()
        public ResultCode GetGroupInfo(ServiceCtx context)
        {
            var groupInfoBuf = context.Request.RecvListBuff[0];
            context.Memory.Write(groupInfoBuf.Position, _groupInfo);

            Logger.Stub?.PrintStub(LogClass.ServiceLdn);

            return ResultCode.Success;
        }
    }
}
