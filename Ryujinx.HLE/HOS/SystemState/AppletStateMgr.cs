using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System.Collections.Concurrent;

namespace Ryujinx.HLE.HOS.SystemState
{
    class AppletStateMgr
    {
        private IdDictionary _appletResourceUserIds;

        public ConcurrentDictionary<long, AppletContext> Applets { get; }

        public KEvent PopFromGeneralChannelEvent { get; }

        public AppletSession GeneralChannel { get; }

        public AppletStateMgr(KernelContext context)
        {
            _appletResourceUserIds = new();
            Applets = new();
            GeneralChannel = new();
            PopFromGeneralChannelEvent = new KEvent(context);
        }

        public void RegisterNewApplet(long processId, AppletId appletId, AppletProcessLaunchReason launchReason, LibraryAppletContext libraryAppletContext)
        {
            var appletResourceUserId = _appletResourceUserIds.Add(processId);
            Applets.TryAdd(processId, new AppletContext(appletId, processId, appletResourceUserId, launchReason, libraryAppletContext));
        }

        public void PushToGeneralChannel(byte[] data)
        {
            GeneralChannel.Push(data);
            PopFromGeneralChannelEvent.ReadableEvent.Signal();
        }

        public bool TryPopFromGeneralChannel(out byte[] data)
        {
            var generalChannelConsumer = GeneralChannel.GetConsumer();
            return generalChannelConsumer.TryPop(out data);
        }

        public long FindProcessIdByAppletResourceUserId(long appletResourceUserId)
        {
            foreach(var (processId, applet) in Applets)
            {
                if(applet.AppletResourceUserId == appletResourceUserId)
                {
                    return processId;
                }
            }

            return -1;
        }

        public void SendMessagesToAllApplets(params AppletMessage[] messages)
        {
            foreach(var (_, applet) in Applets)
            {
                applet.SendMessages(messages);
            }
        }

        public void SetFocusedApplet(long processId)
        {
            foreach (var (appletProcessId, applet) in Applets)
            {
                Ryujinx.Common.Logging.Logger.Error?.Print(Common.Logging.LogClass.ServiceAm, "Applet " + applet.AppletId + " in focus -> " + (appletProcessId == processId));
                applet.SetFocus(appletProcessId == processId);
            }
        }
    }
}