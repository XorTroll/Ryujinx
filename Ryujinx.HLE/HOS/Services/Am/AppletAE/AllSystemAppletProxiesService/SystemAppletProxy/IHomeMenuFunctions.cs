using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy;
using System;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.SystemAppletProxy
{
    class IHomeMenuFunctions : IpcService
    {
        private int    _channelEventHandle;

        public IHomeMenuFunctions()
        {
        }

        [CommandHipc(10)]
        // RequestToGetForeground()
        public ResultCode RequestToGetForeground(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(11)]
        // LockForeground()
        public ResultCode LockForeground(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(12)]
        // UnlockForeground()
        public ResultCode UnlockForeground(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(21)]
        // GetPopFromGeneralChannelEvent() -> handle<copy>
        public ResultCode GetPopFromGeneralChannelEvent(ServiceCtx context)
        {
            if (_channelEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(Horizon.Instance.AppletState.PopFromGeneralChannelEvent.ReadableEvent, out _channelEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_channelEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(31)]
        // GetWriterLockAccessorEx(u32) -> object<nn::am::service::ILockAccessor>
        public ResultCode GetWriterLockAccessorEx(ServiceCtx context)
        {
            var unk = context.RequestData.ReadUInt32();

            Logger.Stub?.PrintStub(LogClass.ServiceAm, new { unk });

            MakeObject(context, new ILockAccessor(unk));

            return ResultCode.Success;
        }
    }
}