using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;

namespace Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy
{
    class ILockAccessor : IpcService
    {
        private uint _unk;
        private KEvent _event;
        private int _eventHandle;

        public ILockAccessor(uint unk)
        {
            _unk = unk;
            _event = new KEvent(Horizon.Instance.KernelContext);
        }

        [CommandHipc(3)]
        // GetEvent() -> handle<copy>
        public ResultCode GetEvent(ServiceCtx context)
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
