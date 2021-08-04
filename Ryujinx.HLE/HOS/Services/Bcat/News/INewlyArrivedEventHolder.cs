using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;

namespace Ryujinx.HLE.HOS.Services.Bcat.News
{
    class INewlyArrivedEventHolder : IpcService
    {
        private KEvent _event;
        private int _eventHandle;

        public INewlyArrivedEventHolder()
        {
            _event = new KEvent(Horizon.Instance.KernelContext);
        }

        [CommandHipc(0)]
        // Get()
        public ResultCode Get(ServiceCtx context)
        {
            if (_eventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_event.ReadableEvent, out _eventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_eventHandle);

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }
    }
}