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
        private KEvent _interactiveOutDataEvent;
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
            _interactiveOutDataEvent = new KEvent(Horizon.Instance.KernelContext);
        }

        public void SetBaseContext(AppletContext context)
        {
            _baseContext = context;
        }

        public void PushInData(byte[] data, bool interactive)
        {
            (interactive ? _interactiveSession : _normalSession).Push(data);
        }

        public bool TryPopOutData(out byte[] data, bool interactive)
        {
            if ((interactive ? _interactiveSession : _normalSession).TryPop(out data))
            {
                (interactive ? _interactiveOutDataEvent : _normalOutDataEvent).ReadableEvent.Clear();
                return true;
            }

            return false;
        }

        public bool TryPopInData(out byte[] data, bool interactive)
        {
            var consumerSession = (interactive ? _interactiveSession : _normalSession).GetConsumer();
            return consumerSession.TryPop(out data);
        }

        public void PushOutData(byte[] data, bool interactive)
        {
            var consumerSession = (interactive ? _interactiveSession : _normalSession).GetConsumer();
            consumerSession.Push(data);
            Ryujinx.Common.Logging.Logger.Warning?.Print(Common.Logging.LogClass.ServiceAm, "Pushing out data from applet " + AppletId + " of size " + data.Length);
            (interactive ? _interactiveOutDataEvent : _normalOutDataEvent).ReadableEvent.Signal();
        }

        public void NotifyStateChanged()
        {
            _stateChangedEvent.ReadableEvent.Signal();
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

        public bool TryCreatePopInteractiveOutDataEventHandle(KProcess process, out int handle)
        {
            return TryCreateEventHandle(process, _interactiveOutDataEvent, out handle);
        }

        public void Terminate()
        {
            Ryujinx.Common.Logging.Logger.Error?.Print(Common.Logging.LogClass.ServiceAm, "Terminating applet " + AppletId);

            // Change state
            NotifyStateChanged();

            // Set caller applet focused
            Horizon.Instance.AppletState.SetFocusedApplet(CallerProcessId);

            // Terminate process
            if (Horizon.Instance.KernelContext.Processes.TryGetValue(_baseContext.ProcessId, out var process))
            {
                process.Terminate();
            }
        }
    }
}
