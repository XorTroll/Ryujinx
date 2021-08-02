using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Ldn.Lp2p
{
    class IMonitorService : IpcService
    {
        public IMonitorService(ServiceCtx context) { }

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
            // TODO: do this nicely
            var group_info = new byte[0x200];
            var group_info_buf = context.Request.RecvListBuff[0];
            context.Memory.Write(group_info_buf.Position, group_info);

            Logger.Stub?.PrintStub(LogClass.ServiceLdn);

            return ResultCode.Success;
        }
    }
}
