using Ryujinx.HLE.FileSystem.Content;
using Ryujinx.HLE.HOS.Services.Caps;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System.Collections.Concurrent;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    public class AppletContext
    {
        private AppletSession _contextSession;
        private AppletContextBase _contextBase;

        public AppletId AppletId { get; private set; }

        public long ProcessId { get; private set; }

        public long AppletResourceUserId { get; private set; }

        public LibraryAppletContext LibraryAppletContext => _contextBase as LibraryAppletContext;

        public ApplicationContext ApplicationContext => _contextBase as ApplicationContext;

        public ConcurrentQueue<AppletMessage> Messages { get; private set; }

        public KEvent MessageEvent { get; private set; }

        public KEvent LibraryAppletLaunchableEvent { get; private set; }

        public FocusState FocusState { get; private set; }

        public AppletProcessLaunchReason LaunchReason { get; private set; }

        public ScreenShotPermission ScreenShotPermission { get; set; }

        public bool OperationModeChangedNotification { get; set; }

        public bool PerformanceModeChangedNotification { get; set; }

        public bool FocusHandlingModeFlag1 { get; set; }

        public bool FocusHandlingModeFlag2 { get; set; }

        public bool FocusHandlingModeFlag3 { get; set; }

        public bool RestartMessageEnabled { get; set; }

        public bool OutOfFocusSuspendingEnabled { get; set; }

        public AlbumImageOrientation ScreenShotImageOrientation { get; set; }

        public bool AutoSleepDisabled { get; set; }

        public bool AlbumImageTakenNotificationEnabled { get; set; }

        public static ulong GetProgramIdFromAppletId(AppletId appletId, bool onlyLibraryAppletsAllowed)
        {
            return appletId switch
            {
                AppletId.OverlayApplet => onlyLibraryAppletsAllowed ? 0x0 : SystemProgramIds.Applets.OverlayDisp,
                AppletId.SystemAppletMenu => onlyLibraryAppletsAllowed ? 0x0 : SystemProgramIds.Applets.Qlaunch,
                AppletId.LibraryAppletController => SystemProgramIds.Applets.Controller,
                AppletId.LibraryAppletSwkbd => SystemProgramIds.Applets.Swkbd,
                AppletId.LibraryAppletMiiEdit => SystemProgramIds.Applets.MiiEdit,
                AppletId.LibAppletShop => SystemProgramIds.Applets.Shop,
                AppletId.LibraryAppletPhotoViewer => SystemProgramIds.Applets.PhotoViewer,
                AppletId.LibraryAppletMyPage =>SystemProgramIds.Applets.MyPage,
                // TODO: add more
                _ => 0x0
            };
        }

        private void Initialize(AppletId appletId, long processId, long appletResourceUserId, AppletProcessLaunchReason launchReason)
        {
            AppletId = appletId;
            ProcessId = processId;
            AppletResourceUserId = appletResourceUserId;
            Messages = new ConcurrentQueue<AppletMessage>();
            MessageEvent = new KEvent(Horizon.Instance.KernelContext);
            LibraryAppletLaunchableEvent = new KEvent(Horizon.Instance.KernelContext);
            FocusState = FocusState.OutOfFocus;
            LaunchReason = launchReason;
            ScreenShotPermission = ScreenShotPermission.Enable;
            OperationModeChangedNotification = true;
            PerformanceModeChangedNotification = true;
            FocusHandlingModeFlag1 = false;
            FocusHandlingModeFlag2 = false;
            FocusHandlingModeFlag3 = false;
            RestartMessageEnabled = false;
            OutOfFocusSuspendingEnabled = false;
            ScreenShotImageOrientation = AlbumImageOrientation.Degrees0;
            AutoSleepDisabled = true;
            AlbumImageTakenNotificationEnabled = true;
            _contextSession = new AppletSession();
        }
        
        public AppletContext(AppletId appletId, long processId, long appletResourceUserId, AppletProcessLaunchReason launchReason)
        {
            Initialize(appletId, processId, appletResourceUserId, launchReason);
        }
        
        public AppletContext(long processId, long appletResourceUserId, AppletProcessLaunchReason launchReason, LibraryAppletContext libraryAppletContext)
        {
            Initialize(libraryAppletContext.AppletId, processId, appletResourceUserId, launchReason);

            _contextBase = libraryAppletContext;
            _contextBase.SetBaseContext(this);
        }

        public AppletContext(long processId, long appletResourceUserId, AppletProcessLaunchReason launchReason, ApplicationContext applicationContext)
        {
            Initialize(applicationContext.AppletId, processId, appletResourceUserId, launchReason);

            _contextBase = applicationContext;
            _contextBase.SetBaseContext(this);
        }

        public bool IsLibraryApplet => _contextBase is LibraryAppletContext;

        public bool IsAnyApplication => _contextBase is ApplicationContext;

        public bool IsApplication => IsAnyApplication && !ApplicationContext.IsSystem;

        public bool IsSystemApplication => IsAnyApplication && ApplicationContext.IsSystem;

        public bool IsSystemApplet => (_contextBase == null) && (AppletId == AppletId.SystemAppletMenu);

        public bool IsOverlayApplet => (_contextBase == null) && (AppletId == AppletId.OverlayApplet);

        public void SendMessages(params AppletMessage[] messages)
        {
            foreach (var message in messages)
            {
                Messages.Enqueue(message);
            }
            MessageEvent.ReadableEvent.Signal();
        }

        public void SetFocus(bool isFocused)
        {
            var focusState = isFocused ? FocusState.InFocus : FocusState.OutOfFocus;

            if (focusState != FocusState)
            {
                FocusState = focusState;

                SendMessages(AppletMessage.FocusStateChanged, isFocused ? AppletMessage.ChangeIntoForeground : AppletMessage.ChangeIntoBackground);
            }
        }

        public void PushContext(byte[] data)
        {
            _contextSession.Push(data);
        }

        public bool TryPopContext(out byte[] data)
        {
            return _contextSession.TryPop(out data);
        }

        public void NotifyLibraryAppletLaunchable()
        {
            LibraryAppletLaunchableEvent.ReadableEvent.Signal();
        }
    }
}
