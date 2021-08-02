using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;

namespace Ryujinx.HLE.HOS.Services.BluetoothManager.BtmSystem
{
    class IBtmSystemCore : IpcService
    {
        private KEvent _radioEvent;
        private int _radioEventHandle;

        public IBtmSystemCore(ServiceCtx context)
        {
            _radioEvent = new KEvent(context.Device.System.KernelContext);
        }

        [CommandHipc(6)]
        // GetRadioOnOff() -> bool
        public ResultCode GetRadioOnOff(ServiceCtx context)
        {
            context.ResponseData.Write(false);

            return ResultCode.Success;
        }

        [CommandHipc(7)]
        // AcquireRadioEvent() -> bool, handle<copy>
        public ResultCode AcquireRadioEvent(ServiceCtx context)
        {
            context.ResponseData.Write(true);

            if (_radioEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_radioEvent.ReadableEvent, out _radioEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_radioEventHandle);

            return ResultCode.Success;
        }
    }
}
