using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Kernel.Process;
using System;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    public abstract class AppletContextBase
    {
        private KEvent _stateChangedEvent;
        private AppletContext _baseContext;

        public long CallerProcessId { get; private set; }

        public AppletId AppletId { get; private set; }

        public uint Result { get; set; }

        public bool IsCompleted { get; private set; }

        public AppletContextBase(long callerProcessId, AppletId appletId)
        {
            CallerProcessId = callerProcessId;
            AppletId = appletId;
            Result = 0;
            _stateChangedEvent = new KEvent(Horizon.Instance.KernelContext);
        }

        public void SetBaseContext(AppletContext context)
        {
            _baseContext = context;
        }

        public void NotifyStateChanged()
        {
            _stateChangedEvent.WritableEvent.Signal();
        }

        protected bool TryCreateEventHandle(KProcess process, KEvent evt, out int handle)
        {
            return process.HandleTable.GenerateHandle(evt.ReadableEvent, out handle) == KernelResult.Success;
        }

        public bool TryCreateStateChangedEventHandle(KProcess process, out int handle)
        {
            return TryCreateEventHandle(process, _stateChangedEvent, out handle);
        }

        public abstract ResultCode Start();

        public void Terminate()
        {
            Ryujinx.Common.Logging.Logger.Error?.Print(Common.Logging.LogClass.ServiceAm, "Terminating applet " + AppletId);

            // Change state
            NotifyStateChanged();

            Horizon.Instance.AppletState.TerminateApplet(_baseContext.AppletResourceUserId);

            // Set caller applet focused
            Horizon.Instance.AppletState.SetFocusedApplet(CallerProcessId, true);

            // Remove the HID shmem the applet may have created
            Horizon.Instance.RemoveHidSharedMemory(_baseContext.AppletResourceUserId);

            // Terminate process
            if (Horizon.Instance.KernelContext.Processes.TryRemove(_baseContext.ProcessId, out var process))
            {
                process.Terminate();
                process.DecrementReferenceCount();
            }
        }
    }
}
