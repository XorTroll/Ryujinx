using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using System;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    class IAccessorBase : DisposableIpcService
    {
        protected AppletContext _self;
        protected AppletContextBase _contextBase;
        private int _stateChangedEventHandle;

        public IAccessorBase(AppletContext self, AppletContextBase contextBase)
        {
            _self = self;
            _contextBase = contextBase;
        }

        [CommandHipc(0)]
        // GetAppletStateChangedEvent() -> handle<copy>
        public ResultCode GetAppletStateChangedEvent(ServiceCtx context)
        {
            if (_stateChangedEventHandle == 0)
            {
                if (!_contextBase.TryCreateStateChangedEventHandle(context.Process, out _stateChangedEventHandle))
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_stateChangedEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(1)]
        // IsCompleted() -> bool
        public ResultCode IsCompleted(ServiceCtx context)
        {
            context.ResponseData.Write(_contextBase.IsCompleted);

            return ResultCode.Success;
        }

        [CommandHipc(10)]
        // Start()
        public ResultCode Start(ServiceCtx context)
        {
            return _contextBase.Start();
        }

        [CommandHipc(20)]
        // RequestExit()
        public ResultCode RequestExit(ServiceCtx context)
        {
            _contextBase.Terminate();

            return ResultCode.Success;
        }

        [CommandHipc(25)]
        // Terminate()
        public ResultCode Terminate(ServiceCtx context)
        {
            _contextBase.Terminate();

            return ResultCode.Success;
        }

        [CommandHipc(30)]
        // GetResult()
        public ResultCode GetResult(ServiceCtx context)
        {
            return (ResultCode)_contextBase.Result;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_stateChangedEventHandle != 0)
                {
                    Horizon.Instance.KernelContext.Syscall.CloseHandle(_stateChangedEventHandle);
                }
            }
        }
    }
}
