using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;

namespace Ryujinx.HLE.HOS.Services.Glue.Notif
{
    class INotificationSystemEventAccessor : IpcService
    {
        private KEvent _event;
        private int _eventHandle;

        public INotificationSystemEventAccessor(KEvent evt)
        {
            _event = evt;
        }

        [CommandHipc(0)]
        // GetEventHandle() -> handle<copy>
        public ResultCode GetEventHandle(ServiceCtx context)
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
