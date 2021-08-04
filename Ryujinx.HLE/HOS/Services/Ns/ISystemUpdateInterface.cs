using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services.Ns.Types;
using System;

namespace Ryujinx.HLE.HOS.Services.Ns
{
    [Service("ns:su")]
    class ISystemUpdateInterface : IpcService
    {
        private KEvent _systemUpdateContentDeliveryNotificationEvent;
        private int _systemUpdateContentDeliveryNotificationEventHandle;

        public ISystemUpdateInterface()
        {
            _systemUpdateContentDeliveryNotificationEvent = new KEvent(Horizon.Instance.KernelContext);
        }

        [CommandHipc(0)]
        // GetBackgroundNetworkUpdateState() -> nn::ns::BackgroundNetworkUpdateState
        public ResultCode GetBackgroundNetworkUpdateState(ServiceCtx context)
        {
            // Not supporting firmware updates fow now...
            context.ResponseData.Write((byte)BackgroundNetworkUpdateState.None);

            Logger.Stub?.PrintStub(LogClass.ServiceNs);

            return ResultCode.Success;
        }

        [CommandHipc(9)]
        // GetSystemUpdateNotificationEventForContentDelivery()
        public ResultCode GetSystemUpdateNotificationEventForContentDelivery(ServiceCtx context)
        {
            if (_systemUpdateContentDeliveryNotificationEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_systemUpdateContentDeliveryNotificationEvent.ReadableEvent, out _systemUpdateContentDeliveryNotificationEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_systemUpdateContentDeliveryNotificationEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceNs);

            return ResultCode.Success;
        }
    }
}