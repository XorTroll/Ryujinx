using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.SystemAppletProxy
{
    class IGlobalStateController : IpcService
    {
        private KEvent _hdcpAuthenticationFailedEvent;
        private int _hdcpAuthenticationFailedEventHandle;

        public IGlobalStateController()
        {
            _hdcpAuthenticationFailedEvent = new KEvent(Horizon.Instance.KernelContext);
        }

        [CommandHipc(14)]
        // ShouldSleepOnBoot() -> bool
        public ResultCode ShouldSleepOnBoot(ServiceCtx context)
        {
            // TODO

            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(15)]
        // GetHdcpAuthenticationFailedEvent() -> handle<copy>
        public ResultCode GetHdcpAuthenticationFailedEvent(ServiceCtx context)
        {
            if (_hdcpAuthenticationFailedEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_hdcpAuthenticationFailedEvent.ReadableEvent, out _hdcpAuthenticationFailedEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_hdcpAuthenticationFailedEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }
    }
}