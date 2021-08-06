using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Ptm.Lbl
{
    abstract class ILblController : IpcService
    {
        protected abstract void SetCurrentBrightnessSettingForVrMode(float currentBrightnessSettingForVrMode);

        protected abstract float GetCurrentBrightnessSettingForVrMode();

        internal abstract void EnableVrMode();

        internal abstract void DisableVrMode();

        protected abstract bool IsVrModeEnabled();

        [CommandHipc(0)]
        // SaveCurrentSetting()
        public ResultCode SaveCurrentSetting(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }

        [CommandHipc(1)]
        // LoadCurrentSetting()
        public ResultCode LoadCurrentSetting(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }

        [CommandHipc(2)]
        // SetCurrentBrightnessSetting(float)
        public ResultCode SetCurrentBrightnessSetting(ServiceCtx context)
        {
            Horizon.Instance.State.Brightness = context.RequestData.ReadSingle();

            return ResultCode.Success;
        }

        [CommandHipc(3)]
        // GetCurrentBrightnessSetting() -> float
        public ResultCode GetCurrentBrightnessSetting(ServiceCtx context)
        {
            context.ResponseData.Write(Horizon.Instance.State.Brightness);

            return ResultCode.Success;
        }

        [CommandHipc(12)]
        // EnableAutoBrightnessControl()
        public ResultCode EnableAutoBrightnessControl(ServiceCtx context)
        {
            Horizon.Instance.State.AutoBrightnessControlEnabled = true;

            return ResultCode.Success;
        }

        [CommandHipc(13)]
        // DisableAutoBrightnessControl()
        public ResultCode DisableAutoBrightnessControl(ServiceCtx context)
        {
            Horizon.Instance.State.AutoBrightnessControlEnabled = false;

            return ResultCode.Success;
        }

        [CommandHipc(14)]
        // IsAutoBrightnessControlEnabled() -> bool
        public ResultCode IsAutoBrightnessControlEnabled(ServiceCtx context)
        {
            context.ResponseData.Write(Horizon.Instance.State.AutoBrightnessControlEnabled);

            return ResultCode.Success;
        }

        [CommandHipc(17)]
        // SetBrightnessReflectionDelayLevel(float, float)
        public ResultCode SetBrightnessReflectionDelayLevel(ServiceCtx context)
        {
            return ResultCode.Success;
        }

        [CommandHipc(18)]
        // GetBrightnessReflectionDelayLevel(float) -> float
        public ResultCode GetBrightnessReflectionDelayLevel(ServiceCtx context)
        {
            context.ResponseData.Write(0.0f);

            return ResultCode.Success;
        }

        [CommandHipc(19)]
        // SetCurrentBrightnessMapping()
        public ResultCode SetCurrentBrightnessMapping(ServiceCtx context)
        {
            // TODO

            var brightness_map = context.RequestData.ReadBytes(0xC);

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }

        [CommandHipc(20)]
        // GetCurrentBrightnessMapping()
        public ResultCode GetCurrentBrightnessMapping(ServiceCtx context)
        {
            // TODO

            var brightness_map = new byte[0xC];
            context.ResponseData.Write(brightness_map);

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }

        [CommandHipc(21)]
        // SetCurrentAmbientLightSensorMapping(unknown<0xC>)
        public ResultCode SetCurrentAmbientLightSensorMapping(ServiceCtx context)
        {
            return ResultCode.Success;
        }

        [CommandHipc(22)]
        // GetCurrentAmbientLightSensorMapping() -> unknown<0xC>
        public ResultCode GetCurrentAmbientLightSensorMapping(ServiceCtx context)
        {
            return ResultCode.Success;
        }

        [CommandHipc(24)] // 3.0.0+
        // SetCurrentBrightnessSettingForVrMode(float)
        public ResultCode SetCurrentBrightnessSettingForVrMode(ServiceCtx context)
        {
            float currentBrightnessSettingForVrMode = context.RequestData.ReadSingle();

            SetCurrentBrightnessSettingForVrMode(currentBrightnessSettingForVrMode);

            return ResultCode.Success;
        }

        [CommandHipc(25)] // 3.0.0+
        // GetCurrentBrightnessSettingForVrMode() -> float
        public ResultCode GetCurrentBrightnessSettingForVrMode(ServiceCtx context)
        {
            float currentBrightnessSettingForVrMode = GetCurrentBrightnessSettingForVrMode();

            context.ResponseData.Write(currentBrightnessSettingForVrMode);

            return ResultCode.Success;
        }

        [CommandHipc(26)] // 3.0.0+
        // EnableVrMode()
        public ResultCode EnableVrMode(ServiceCtx context)
        {
            EnableVrMode();

            return ResultCode.Success;
        }

        [CommandHipc(27)] // 3.0.0+
        // DisableVrMode()
        public ResultCode DisableVrMode(ServiceCtx context)
        {
            DisableVrMode();

            return ResultCode.Success;
        }

        [CommandHipc(28)] // 3.0.0+
        // IsVrModeEnabled() -> bool
        public ResultCode IsVrModeEnabled(ServiceCtx context)
        {
            context.ResponseData.Write(IsVrModeEnabled());

            return ResultCode.Success;
        }
    }
}