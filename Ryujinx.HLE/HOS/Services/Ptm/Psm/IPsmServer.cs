using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Ptm.Psm
{
    [Service("psm")]
    class IPsmServer : IpcService
    {
        [CommandHipc(0)]
        // GetBatteryChargePercentage() -> u32
        public static ResultCode GetBatteryChargePercentage(ServiceCtx context)
        {
            int chargePercentage = 100;

            context.ResponseData.Write(chargePercentage);

            Logger.Stub?.PrintStub(LogClass.ServicePsm, new { chargePercentage });

            return ResultCode.Success;
        }

        [CommandHipc(1)]
        // GetChargerType() -> u32
        public static ResultCode GetChargerType(ServiceCtx context)
        {
            ChargerType chargerType = ChargerType.EnoughPower;

            context.ResponseData.Write((int)chargerType);

            Logger.Stub?.PrintStub(LogClass.ServicePsm, new { chargerType });

            return ResultCode.Success;
        }

        [CommandHipc(7)]
        // OpenSession() -> IPsmSession
        public ResultCode OpenSession(ServiceCtx context)
        {
            MakeObject(context, new IPsmSession());

            return ResultCode.Success;
        }
    }
}