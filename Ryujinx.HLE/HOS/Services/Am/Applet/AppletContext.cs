﻿using Ryujinx.HLE.FileSystem.Content;
using Ryujinx.HLE.HOS.Services.Caps;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System.Collections.Concurrent;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    public class AppletContext
    {
        private AppletSession _contextSession;

        public AppletId AppletId { get; }

        public long ProcessId { get; }

        public long AppletResourceUserId { get; }

        public LibraryAppletContext LibraryAppletContext { get; }

        public ConcurrentQueue<AppletMessage> Messages { get; }

        public KEvent MessageEvent { get; }

        public FocusState FocusState { get; private set; }

        public AppletProcessLaunchReason LaunchReason { get; }

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

        public AppletContext(AppletId appletId, long processId, long appletResourceUserId, AppletProcessLaunchReason launchReason, LibraryAppletContext libraryAppletContext)
        {
            AppletId = appletId;
            ProcessId = processId;
            AppletResourceUserId = appletResourceUserId;
            LibraryAppletContext = libraryAppletContext;
            Messages = new ConcurrentQueue<AppletMessage>();
            MessageEvent = new KEvent(Horizon.Instance.KernelContext);
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

            if(LibraryAppletContext != null)
            {
                LibraryAppletContext.SetBaseContext(this);
            }
        }

        public bool IsLibraryApplet() => LibraryAppletContext != null;

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
    }
}
