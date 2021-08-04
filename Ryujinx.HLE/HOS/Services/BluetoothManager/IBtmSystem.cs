using Ryujinx.HLE.HOS.Services.BluetoothManager.BtmSystem;

namespace Ryujinx.HLE.HOS.Services.BluetoothManager
{
    [Service("btm:sys")]
    class IBtmSystem : IpcService
    {
        [CommandHipc(0)]
        // GetCore() -> object<nn::btm::IBtmSystemCore>
        public ResultCode GetCore(ServiceCtx context)
        {
            MakeObject(context, new IBtmSystemCore());

            return ResultCode.Success;
        }
    }
}