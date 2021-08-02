using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Bcat.News
{
    class INewsDatabaseService : IpcService
    {
        public INewsDatabaseService() { }

        [CommandHipc(0)]
        // GetListV1(u32, buffer, buffer) -> (u32, buffer)
        public ResultCode GetListV1(ServiceCtx context)
        {
            // TODO

            var unk = context.RequestData.ReadUInt32();

            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.Service, new { unk });

            return ResultCode.Success;
        }

        [CommandHipc(1)]
        // Count() -> u32
        public ResultCode Count(ServiceCtx context)
        {
            // TODO

            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }

        [CommandHipc(4)]
        // UpdateIntegerValueWithAddition(u32, buffer, buffer)
        public ResultCode UpdateIntegerValueWithAddition(ServiceCtx context)
        {
            // TODO

            var unk = context.RequestData.ReadUInt32();

            Logger.Stub?.PrintStub(LogClass.Service, new { unk });

            return ResultCode.Success;
        }
    }
}
