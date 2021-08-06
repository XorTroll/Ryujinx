using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Nfc.NfcManager
{
    class INfc : IpcService
    {
        private NfcPermissionLevel _permissionLevel;

        public INfc(NfcPermissionLevel permissionLevel)
        {
            _permissionLevel = permissionLevel;
        }

        [CommandHipc(0)]
        [CommandHipc(400)] // 4.0.0+
        // Initialize()
        public ResultCode Initialize(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceNfc, new { _permissionLevel });

            return ResultCode.Success;
        }

        [CommandHipc(3)]
        // IsNfcEnabledOld() -> bool
        [CommandHipc(403)] // 4.0.0+
        // IsNfcEnabled() -> bool
        public ResultCode IsNfcEnabled(ServiceCtx context)
        {
            // NOTE: Write false value here could make nfp service not called.
            context.ResponseData.Write(Horizon.Instance.State.NfcEnabled);

            return ResultCode.Success;
        }

        [CommandHipc(100)]
        // SetNfcEnabledOld(bool)
        [CommandHipc(500)] // 4.0.0+
        // SetNfcEnabled(bool)
        public ResultCode SetNfcEnabled(ServiceCtx context)
        {
            Horizon.Instance.State.NfcEnabled = context.RequestData.ReadBoolean();

            return ResultCode.Success;
        }
    }
}