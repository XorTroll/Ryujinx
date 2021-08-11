using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Kernel.Process;
using System;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    public class LibraryAppletContext : AppletContextBase
    {
        private AppletSession _normalSession;
        private AppletSession _interactiveSession;
        private KEvent _normalOutDataEvent;
        private KEvent _normalInDataEvent;
        private KEvent _interactiveOutDataEvent;
        private KEvent _interactiveInDataEvent;

        public LibraryAppletMode LibraryAppletMode { get; private set; }

        public LibraryAppletContext(long callerProcessId, AppletId appletId, LibraryAppletMode libraryAppletMode) : base(callerProcessId, appletId)
        {
            LibraryAppletMode = libraryAppletMode;
            _normalSession = new();
            _interactiveSession = new();
            _normalOutDataEvent = new KEvent(Horizon.Instance.KernelContext);
            _normalInDataEvent = new KEvent(Horizon.Instance.KernelContext);
            _interactiveOutDataEvent = new KEvent(Horizon.Instance.KernelContext);
            _interactiveInDataEvent = new KEvent(Horizon.Instance.KernelContext);
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

        public override ResultCode Start()
        {
            // Logger.Info?.Print(LogClass.ServiceAm, "Starting applet...");

            var processId = Horizon.Instance.Device.Application.LoadLibraryApplet(this);
            if (processId < 0)
            {
                return ResultCode.AppletLaunchFailed;
            }
            else
            {
                return ResultCode.Success;
            }
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
    }
}
