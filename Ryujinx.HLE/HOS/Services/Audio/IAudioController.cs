using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Audio
{
    [Service("audctl")]
    class IAudioController : IpcService
    {
        [CommandHipc(12)]
        // GetForceMutePolicy() -> u32
        public ResultCode GetForceMutePolicy(ServiceCtx context)
        {
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.Audio);

            return ResultCode.Success;
        }

        [CommandHipc(13)]
        // GetOutputModeSetting() -> u32
        public ResultCode GetOutputModeSetting(ServiceCtx context)
        {
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.Audio);

            return ResultCode.Success;
        }

        [CommandHipc(18)]
        // GetHeadphoneOutputLevelMode() -> u32
        public ResultCode GetHeadphoneOutputLevelMode(ServiceCtx context)
        {
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.Audio);

            return ResultCode.Success;
        }
    }
}