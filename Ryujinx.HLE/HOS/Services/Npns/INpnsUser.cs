using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;

namespace Ryujinx.HLE.HOS.Services.Npns
{
    [Service("npns:u")]
    class INpnsUser : IpcService
    {
        public KEvent _receiveEvent;
        public int _receiveEventHandle;

        public INpnsUser(KernelContext context)
        {
            _receiveEvent = new KEvent(context);
        }

        [CommandHipc(2)]
        // ListenTo(u64)
        public ResultCode ListenTo(ServiceCtx context)
        {
            var programId = context.RequestData.ReadUInt64();

            Logger.Stub?.PrintStub(LogClass.Service, new { programId });

            return ResultCode.Success;
        }

        [CommandHipc(5)]
        // GetReceiveEvent() -> handle<copy>
        public ResultCode GetReceiveEvent(ServiceCtx context)
        {
            if (_receiveEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_receiveEvent.ReadableEvent, out _receiveEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_receiveEventHandle);

            return ResultCode.Success;
        }
    }
}