using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using System;

namespace Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy.LibraryAppletCreator
{
    class ILibraryAppletAccessor : IAccessorBase
    {
        private int _normalOutDataEventHandle;
        private int _interactiveOutDataEventHandle;
        
        private LibraryAppletContext _context => _contextBase as LibraryAppletContext;

        public ILibraryAppletAccessor(AppletContext self, AppletId appletId, LibraryAppletMode libraryAppletMode) : base(self, new LibraryAppletContext(self.ProcessId, appletId, libraryAppletMode))
        {
            Logger.Info?.Print(LogClass.ServiceAm, $"Applet '{appletId}' created...");
        }

        // NOTE: Commands 0, 1, 10, 20, 25 and 30 are implemented in IAccessorBase

        [CommandHipc(60)]
        // PresetLibraryAppletGpuTimeSliceZero()
        public ResultCode PresetLibraryAppletGpuTimeSliceZero(ServiceCtx context)
        {
            // NOTE: This call reset two internal fields to 0 and one internal field to "true".
            //       It seems to be used only with software keyboard inline.
            //       Since we doesn't support applets for now, it's fine to stub it.

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(100)]
        // PushInData(object<nn::am::service::IStorage>)
        public ResultCode PushInData(ServiceCtx context)
        {
            var data = GetObject<IStorage>(context, 0);

            _context.PushInData(data.Data, false);

            return ResultCode.Success;
        }

        [CommandHipc(101)]
        // PopOutData() -> object<nn::am::service::IStorage>
        public ResultCode PopOutData(ServiceCtx context)
        {
            if (_context.TryPopOutData(out var data, false))
            {
                MakeObject(context, new IStorage(data));

                return ResultCode.Success;
            }
            else
            {
                return ResultCode.NotAvailable;
            }
        }

        [CommandHipc(103)]
        // PushInteractiveInData(object<nn::am::service::IStorage>)
        public ResultCode PushInteractiveInData(ServiceCtx context)
        {
            var data = GetObject<IStorage>(context, 0);

            _context.PushInData(data.Data, true);

            return ResultCode.Success;
        }

        [CommandHipc(104)]
        // PopInteractiveOutData() -> object<nn::am::service::IStorage>
        public ResultCode PopInteractiveOutData(ServiceCtx context)
        {
            if (_context.TryPopOutData(out byte[] data, true))
            {
                MakeObject(context, new IStorage(data));

                return ResultCode.Success;
            }

            return ResultCode.NotAvailable;
        }

        [CommandHipc(105)]
        // GetPopOutDataEvent() -> handle<copy>
        public ResultCode GetPopOutDataEvent(ServiceCtx context)
        {
            if (_normalOutDataEventHandle == 0)
            {
                if (!_context.TryCreatePopOutDataEventHandle(context.Process, out _normalOutDataEventHandle))
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_normalOutDataEventHandle);

            return ResultCode.Success;
        }

        [CommandHipc(106)]
        // GetPopInteractiveOutDataEvent() -> handle<copy>
        public ResultCode GetPopInteractiveOutDataEvent(ServiceCtx context)
        {
            if (_interactiveOutDataEventHandle == 0)
            {
                if (!_context.TryCreatePopInteractiveOutDataEventHandle(context.Process, out _interactiveOutDataEventHandle))
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_interactiveOutDataEventHandle);

            return ResultCode.Success;
        }

        [CommandHipc(110)]
        // NeedsToExitProcess()
        public ResultCode NeedsToExitProcess(ServiceCtx context)
        {
            return ResultCode.Stubbed;
        }

        [CommandHipc(150)]
        // RequestForAppletToGetForeground()
        public ResultCode RequestForAppletToGetForeground(ServiceCtx context)
        {
            return ResultCode.Stubbed;
        }

        [CommandHipc(160)] // 2.0.0+
        // GetIndirectLayerConsumerHandle() -> u64 indirect_layer_consumer_handle
        public ResultCode GetIndirectLayerConsumerHandle(ServiceCtx context)
        {
            /*
            if (indirectLayerConsumer == null)
            {
                return ResultCode.ObjectInvalid;
            }
            */

            // TODO: Official sw uses this during LibraryApplet creation when LibraryAppletMode is 0x3.
            //       Since we don't support IndirectLayer and the handle couldn't be 0, it's fine to return 1.

            ulong indirectLayerConsumerHandle = 1;

            context.ResponseData.Write(indirectLayerConsumerHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceAm, new { indirectLayerConsumerHandle });

            return ResultCode.Success;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_normalOutDataEventHandle != 0)
                {
                    Horizon.Instance.KernelContext.Syscall.CloseHandle(_normalOutDataEventHandle);
                }

                if (_interactiveOutDataEventHandle != 0)
                {
                    Horizon.Instance.KernelContext.Syscall.CloseHandle(_interactiveOutDataEventHandle);
                }
            }
        }
    }
}
