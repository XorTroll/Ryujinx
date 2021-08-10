using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Kernel.Process;
using System;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    public class LibraryAppletContext
    {
        private AppletSession _normalSession;
        private AppletSession _interactiveSession;
        private KEvent _stateChangedEvent;
        private KEvent _normalOutDataEvent;
        private KEvent _normalInDataEvent;
        private KEvent _interactiveOutDataEvent;
        private KEvent _interactiveInDataEvent;
        private AppletContext _baseContext;

        public long CallerProcessId { get; private set; }

        public AppletId AppletId => _baseContext.AppletId;

        public LibraryAppletMode LibraryAppletMode { get; private set; }

        public uint Result { get; set; }

        public LibraryAppletContext(long callerProcessId, LibraryAppletMode libraryAppletMode)
        {
            CallerProcessId = callerProcessId;
            LibraryAppletMode = libraryAppletMode;
            Result = 0;
            _normalSession = new();
            _interactiveSession = new();
            _stateChangedEvent = new KEvent(Horizon.Instance.KernelContext);
            _normalOutDataEvent = new KEvent(Horizon.Instance.KernelContext);
            _normalInDataEvent = new KEvent(Horizon.Instance.KernelContext);
            _interactiveOutDataEvent = new KEvent(Horizon.Instance.KernelContext);
            _interactiveInDataEvent = new KEvent(Horizon.Instance.KernelContext);
        }

        public void SetBaseContext(AppletContext context)
        {
            _baseContext = context;
        }

        public void PushInData(byte[] data, bool interactive)
        {
            (interactive ? _interactiveSession : _normalSession).Push(data);
            (interactive ? _interactiveInDataEvent : _normalInDataEvent).WritableEvent.Signal();
        }

        public bool TryPopOutData(out byte[] data, bool interactive)
        {
            if ((interactive ? _interactiveSession : _normalSession).TryPop(out data))
            {
                (interactive ? _interactiveOutDataEvent : _normalOutDataEvent).WritableEvent.Clear();
                return true;
            }

            return false;
        }

        public bool TryPopInData(out byte[] data, bool interactive)
        {
            var consumerSession = (interactive ? _interactiveSession : _normalSession).GetConsumer();
            if(consumerSession.TryPop(out data))
            {
                (interactive ? _interactiveInDataEvent : _normalInDataEvent).WritableEvent.Clear();
                return true;
            }

            return false;
        }

        public void PushOutData(byte[] data, bool interactive)
        {
            var consumerSession = (interactive ? _interactiveSession : _normalSession).GetConsumer();
            consumerSession.Push(data);
            Ryujinx.Common.Logging.Logger.Warning?.Print(Common.Logging.LogClass.ServiceAm, "Pushing out data from applet " + AppletId + " of size " + data.Length);
            (interactive ? _interactiveOutDataEvent : _normalOutDataEvent).WritableEvent.Signal();
        }

        public void NotifyStateChanged()
        {
            _stateChangedEvent.WritableEvent.Signal();
        }

        private bool TryCreateEventHandle(KProcess process, KEvent evt, out int handle)
        {
            return process.HandleTable.GenerateHandle(evt.ReadableEvent, out handle) == KernelResult.Success;
        }

        public bool TryCreateStateChangedEventHandle(KProcess process, out int handle)
        {
            return TryCreateEventHandle(process, _stateChangedEvent, out handle);
        }

        public bool TryCreatePopOutDataEventHandle(KProcess process, out int handle)
        {
            return TryCreateEventHandle(process, _normalOutDataEvent, out handle);
        }

        public bool TryCreatePopInDataEventHandle(KProcess process, out int handle)
        {
            return TryCreateEventHandle(process, _normalInDataEvent, out handle);
        }

        public bool TryCreatePopInteractiveOutDataEventHandle(KProcess process, out int handle)
        {
            return TryCreateEventHandle(process, _interactiveOutDataEvent, out handle);
        }

        public bool TryCreatePopInteractiveInDataEventHandle(KProcess process, out int handle)
        {
            return TryCreateEventHandle(process, _interactiveInDataEvent, out handle);
        }

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
