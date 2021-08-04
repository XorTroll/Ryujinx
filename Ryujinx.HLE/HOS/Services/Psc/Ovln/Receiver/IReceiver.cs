using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;

namespace Ryujinx.HLE.HOS.Services.Psc.Ovln.Receiver
{
    class IReceiver : IpcService
    {
        public KEvent _event;
        public int _eventHandle;

        public IReceiver()
        {
            _event = new KEvent(Horizon.Instance.KernelContext);
        }

        [CommandHipc(0)]
        // AddSource(unk<0x10>)
        public ResultCode AddSource(ServiceCtx context)
        {
            var unk1 = context.RequestData.ReadUInt64();
            var unk2 = context.RequestData.ReadUInt64();

            Logger.Stub?.PrintStub(LogClass.Service, new { unk1, unk2 });

            return ResultCode.Success;
        }

        [CommandHipc(2)]
        // GetReceiveEventHandle() -> handle<copy>
        public ResultCode GetReceiveEventHandle(ServiceCtx context)
        {
            if (_eventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_event.ReadableEvent, out _eventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_eventHandle);

            return ResultCode.Success;
        }
    }
}
