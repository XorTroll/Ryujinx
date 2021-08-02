using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Vi.RootService.ApplicationDisplayService
{
    class ISystemDisplayService : IpcService
    {
        private IApplicationDisplayService _applicationDisplayService;

        public ISystemDisplayService(IApplicationDisplayService applicationDisplayService)
        {
            _applicationDisplayService = applicationDisplayService;
        }

        [CommandHipc(2205)]
        // SetLayerZ(u64, u64)
        public ResultCode SetLayerZ(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceVi);

            return ResultCode.Success;
        }

        [CommandHipc(2207)]
        // SetLayerVisibility(b8, u64)
        public ResultCode SetLayerVisibility(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceVi);

            return ResultCode.Success;
        }

        [CommandHipc(2312)] // 1.0.0-6.2.0
        // CreateStrayLayer(u32, u64) -> (u64, u64, buffer<bytes, 6>)
        public ResultCode CreateStrayLayer(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceVi);

            return _applicationDisplayService.CreateStrayLayer(context);
        }

        [CommandHipc(3000)]
        // ListDisplayModes() -> buffer
        public ResultCode ListDisplayModes(ServiceCtx context)
        {
            var unk_val = context.RequestData.ReadUInt64();
            context.ResponseData.Write((ulong)1);

            var modes_buf = context.Request.ReceiveBuff[0];

            ulong offset = 0;
            context.Memory.Write(modes_buf.Position + offset, DisplayModeInfo.DefaultDisplayMode.Width);
            offset += sizeof(uint);
            context.Memory.Write(modes_buf.Position + offset, DisplayModeInfo.DefaultDisplayMode.Height);
            offset += sizeof(uint);
            context.Memory.Write(modes_buf.Position + offset, DisplayModeInfo.DefaultDisplayMode.Unk);
            offset += sizeof(float);
            context.Memory.Write(modes_buf.Position + offset, DisplayModeInfo.DefaultDisplayMode.Unk2);

            Logger.Stub?.PrintStub(LogClass.ServiceVi, new { unk_val });

            return ResultCode.Success;
        }

        [CommandHipc(3200)]
        // GetDisplayMode(u64) -> nn::vi::DisplayModeInfo
        public ResultCode GetDisplayMode(ServiceCtx context)
        {
            // TODO: De-hardcode resolution.
            context.ResponseData.Write(DisplayModeInfo.DefaultDisplayMode.Width);
            context.ResponseData.Write(DisplayModeInfo.DefaultDisplayMode.Height);
            context.ResponseData.Write(DisplayModeInfo.DefaultDisplayMode.Unk);
            context.ResponseData.Write(DisplayModeInfo.DefaultDisplayMode.Unk2);

            return ResultCode.Success;
        }
    }
}