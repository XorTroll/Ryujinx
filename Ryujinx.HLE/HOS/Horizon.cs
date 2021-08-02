using LibHac;
using LibHac.Fs;
using LibHac.FsSystem;
using Ryujinx.Audio;
using Ryujinx.Audio.Input;
using Ryujinx.Audio.Integration;
using Ryujinx.Audio.Output;
using Ryujinx.Audio.Renderer.Device;
using Ryujinx.HLE.FileSystem.Content;
using Ryujinx.HLE.HOS.Font;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Memory;
using Ryujinx.HLE.HOS.Kernel.Process;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services;
using Ryujinx.HLE.HOS.Services.Account.Acc;
using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Glue.Arp;
using Ryujinx.HLE.HOS.Services.Audio.AudioRenderer;
using Ryujinx.HLE.HOS.Services.Sdb.Mii;
using Ryujinx.HLE.HOS.Services.Nfc.Nfp.NfpManager;
using Ryujinx.HLE.HOS.Services.Nv.NvDrvServices.NvHostCtrl;
using Ryujinx.HLE.HOS.Services.Pcv.Bpc;
using Ryujinx.HLE.HOS.Services.Ptm.Apm;

using Ryujinx.HLE.HOS.Services.Account;
using Ryujinx.HLE.HOS.Services.Am;
using Ryujinx.HLE.HOS.Services.Audio;
using Ryujinx.HLE.HOS.Services.Bcat;
using Ryujinx.HLE.HOS.Services.Bluetooth;
using Ryujinx.HLE.HOS.Services.BluetoothManager;
using Ryujinx.HLE.HOS.Services.Caps;
using Ryujinx.HLE.HOS.Services.Erpt;
using Ryujinx.HLE.HOS.Services.Es;
using Ryujinx.HLE.HOS.Services.Eupld;
using Ryujinx.HLE.HOS.Services.Fatal;
using Ryujinx.HLE.HOS.Services.Friend;
using Ryujinx.HLE.HOS.Services.Fs;
using Ryujinx.HLE.HOS.Services.Glue;
using Ryujinx.HLE.HOS.Services.Grc;
using Ryujinx.HLE.HOS.Services.Hid;
using Ryujinx.HLE.HOS.Services.Ldn;
using Ryujinx.HLE.HOS.Services.Lm;
using Ryujinx.HLE.HOS.Services.Loader;
using Ryujinx.HLE.HOS.Services.Mig;
using Ryujinx.HLE.HOS.Services.Ncm;
using Ryujinx.HLE.HOS.Services.Nfc;
using Ryujinx.HLE.HOS.Services.Ngct;
using Ryujinx.HLE.HOS.Services.Nifm;
using Ryujinx.HLE.HOS.Services.Nim;
using Ryujinx.HLE.HOS.Services.Npns;
using Ryujinx.HLE.HOS.Services.Ns;
using Ryujinx.HLE.HOS.Services.Nv;
using Ryujinx.HLE.HOS.Services.Olsc;
using Ryujinx.HLE.HOS.Services.Pcie;
using Ryujinx.HLE.HOS.Services.Pctl;
using Ryujinx.HLE.HOS.Services.Pcv;
using Ryujinx.HLE.HOS.Services.Pm;
using Ryujinx.HLE.HOS.Services.Psc;
using Ryujinx.HLE.HOS.Services.Ptm;
using Ryujinx.HLE.HOS.Services.Ro;
using Ryujinx.HLE.HOS.Services.Sdb;
using Ryujinx.HLE.HOS.Services.Settings;
using Ryujinx.HLE.HOS.Services.Sm;
using Ryujinx.HLE.HOS.Services.Sockets;
using Ryujinx.HLE.HOS.Services.Spl;
using Ryujinx.HLE.HOS.Services.Ssl;
using Ryujinx.HLE.HOS.Services.SurfaceFlinger;
using Ryujinx.HLE.HOS.Services.Usb;
using Ryujinx.HLE.HOS.Services.Vi;
using Ryujinx.HLE.HOS.Services.Wlan;

using Ryujinx.HLE.HOS.Services.Time.Clock;
using Ryujinx.HLE.HOS.SystemState;
using Ryujinx.HLE.Loaders.Executables;
using Ryujinx.HLE.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using AudioRendererManager = Ryujinx.Audio.Renderer.Server.AudioRendererManager;
using LibHacBcatServer = LibHac.Bcat.BcatServer;

namespace Ryujinx.HLE.HOS
{
    using TimeServiceManager = Services.Time.TimeManager;

    public class Horizon : IDisposable
    {
        internal const int HidSize                 = 0x40000;
        internal const int FontSize                = 0x1100000;
        internal const int IirsSize                = 0x8000;
        internal const int TimeSize                = 0x1000;
        internal const int AppletCaptureBufferSize = 0x384000;

        internal KernelContext KernelContext { get; }

        internal Switch Device { get; private set; }

        internal SurfaceFlinger SurfaceFlinger { get; private set; }
        internal AudioManager AudioManager { get; private set; }
        internal AudioOutputManager AudioOutputManager { get; private set; }
        internal AudioInputManager AudioInputManager { get; private set; }
        internal AudioRendererManager AudioRendererManager { get; private set; }
        internal VirtualDeviceSessionRegistry AudioDeviceSessionRegistry { get; private set; }

        public SystemStateMgr State { get; private set; }

        internal PerformanceState PerformanceState { get; private set; }

        internal AppletStateMgr AppletState { get; private set; }

        internal List<NfpDevice> NfpDevices { get; private set; }

        internal SmServer Sm { get; private set; }
        internal SocketsServer Bsdsockets { get; private set; }
        internal AccountServer Account { get; private set; }
        internal FsServer Fs { get; private set; }
        internal LmServer LogManager { get; private set; }
        internal NcmServer Ncm { get; private set; }
        internal PmServer ProcessMana { get; private set; }
        internal UsbServer Usb { get; private set; }
        internal SettingsServer Settings { get; private set; }
        internal BluetoothServer Bluetooth { get; private set; }
        internal BtmServer Btm { get; private set; }
        internal FriendsServer Friends { get; private set; }
        internal PtmServer Ptm { get; private set; }
        internal HidServer Hid { get; private set; }
        internal AudioServer Audio { get; private set; }
        internal WlanServer Wlan { get; private set; }
        internal LdnServer Ldn { get; private set; }
        internal NvServer Nvservices { get; private set; }
        internal PcvServer Pcv { get; private set; }
        internal NvnServer Nvnflinger { get; private set; }
        internal PcieServer Pcie { get; private set; }
        internal NsServer Ns { get; private set; }
        internal NfcServer Nfc { get; private set; }
        internal PscServer Psc { get; private set; }
        internal CapsServer Capsrv { get; private set; }
        internal AmServer Am { get; private set; }
        internal SslServer Ssl { get; private set; }
        internal NimServer Nim { get; private set; }
        internal SplServer Spl { get; private set; }
        internal ErptServer Erpt { get; private set; }
        internal PctlServer Pctl { get; private set; }
        internal NpnsServer Npns { get; private set; }
        internal EupldServer Eupld { get; private set; }
        internal GlueServer Glue { get; private set; }
        internal EsServer Es { get; private set; }
        internal FatalServer Fatal { get; private set; }
        internal GrcServer Grc { get; private set; }
        internal RoServer Ro { get; private set; }
        internal SdbServer Sdb { get; private set; }
        internal MigServer Migration { get; private set; }
        internal OlscServer Olsc { get; private set; }
        internal LoaderServer Loader { get; private set; }
        internal NgctServer Ngct { get; private set; }
        internal NifmServer Nifm { get; private set; }
        internal ViServer Vi { get; private set; }
        internal BcatServer Bcat { get; private set; }

        internal ServerManager[] GetServerList() => new ServerManager[]
        {
            Sm,
            Bsdsockets,
            Account,
            Fs,
            LogManager,
            Ncm,
            ProcessMana,
            Usb,
            Settings,
            Bluetooth,
            Btm,
            Friends,
            Ptm,
            Hid,
            Audio,
            Wlan,
            Ldn,
            Nvservices,
            Pcv,
            Nvnflinger,
            Pcie,
            Ns,
            Nfc,
            Psc,
            Capsrv,
            Am,
            Ssl,
            Nim,
            Spl,
            Erpt,
            Pctl,
            Npns,
            Eupld,
            Glue,
            Es,
            Fatal,
            Grc,
            Ro,
            Sdb,
            Migration,
            Olsc,
            Loader,
            Ngct,
            Nifm,
            Vi,
            Bcat
        };

        internal KSharedMemory HidSharedMem  { get; private set; }
        internal KSharedMemory FontSharedMem { get; private set; }
        internal KSharedMemory IirsSharedMem { get; private set; }

        internal KTransferMemory AppletCaptureBufferTransfer { get; private set; }

        internal SharedFontManager Font { get; private set; }

        internal AccountManager AccountManager { get; private set; }
        internal ContentManager ContentManager { get; private set; }
        internal CaptureManager CaptureManager { get; private set; }

        internal KEvent VsyncEvent { get; private set; }

        internal KEvent DisplayResolutionChangeEvent { get; private set; }

        public Keyset KeySet => Device.FileSystem.KeySet;

        private bool _isDisposed;

        public bool EnablePtc { get; set; }

        public IntegrityCheckLevel FsIntegrityCheckLevel { get; set; }

        public int GlobalAccessLogMode { get; set; }

        internal SharedMemoryStorage HidStorage { get; private set; }

        internal NvHostSyncpt HostSyncpoint { get; private set; }

        internal LibHac.Horizon LibHacHorizonServer { get; private set; }
        internal HorizonClient LibHacHorizonClient { get; private set; }

        public Horizon(Switch device)
        {
            KernelContext = new KernelContext(
                device,
                device.Memory,
                device.Configuration.MemoryConfiguration.ToKernelMemorySize(),
                device.Configuration.MemoryConfiguration.ToKernelMemoryArrange());

            Device = device;

            State = new SystemStateMgr();

            PerformanceState = new PerformanceState();

            NfpDevices = new List<NfpDevice>();

            // Note: This is not really correct, but with HLE of services, the only memory
            // region used that is used is Application, so we can use the other ones for anything.
            KMemoryRegionManager region = KernelContext.MemoryManager.MemoryRegions[(int)MemoryRegion.NvServices];

            ulong hidPa                 = region.Address;
            ulong fontPa                = region.Address + HidSize;
            ulong iirsPa                = region.Address + HidSize + FontSize;
            ulong timePa                = region.Address + HidSize + FontSize + IirsSize;
            ulong appletCaptureBufferPa = region.Address + HidSize + FontSize + IirsSize + TimeSize;

            KPageList hidPageList                 = new KPageList();
            KPageList fontPageList                = new KPageList();
            KPageList iirsPageList                = new KPageList();
            KPageList timePageList                = new KPageList();
            KPageList appletCaptureBufferPageList = new KPageList();

            hidPageList.AddRange(hidPa, HidSize / KPageTableBase.PageSize);
            fontPageList.AddRange(fontPa, FontSize / KPageTableBase.PageSize);
            iirsPageList.AddRange(iirsPa, IirsSize / KPageTableBase.PageSize);
            timePageList.AddRange(timePa, TimeSize / KPageTableBase.PageSize);
            appletCaptureBufferPageList.AddRange(appletCaptureBufferPa, AppletCaptureBufferSize / KPageTableBase.PageSize);

            var hidStorage = new SharedMemoryStorage(KernelContext, hidPageList);
            var fontStorage = new SharedMemoryStorage(KernelContext, fontPageList);
            var iirsStorage = new SharedMemoryStorage(KernelContext, iirsPageList);
            var timeStorage = new SharedMemoryStorage(KernelContext, timePageList);
            var appletCaptureBufferStorage = new SharedMemoryStorage(KernelContext, appletCaptureBufferPageList);

            HidStorage = hidStorage;

            HidSharedMem  = new KSharedMemory(KernelContext, hidStorage,  0, 0, KMemoryPermission.Read);
            FontSharedMem = new KSharedMemory(KernelContext, fontStorage, 0, 0, KMemoryPermission.Read);
            IirsSharedMem = new KSharedMemory(KernelContext, iirsStorage, 0, 0, KMemoryPermission.Read);

            KSharedMemory timeSharedMemory = new KSharedMemory(KernelContext, timeStorage, 0, 0, KMemoryPermission.Read);

            TimeServiceManager.Instance.Initialize(device, this, timeSharedMemory, timeStorage, TimeSize);

            AppletCaptureBufferTransfer = new KTransferMemory(KernelContext, appletCaptureBufferStorage);

            AppletState = new AppletStateMgr(this);

            AppletState.SetFocus(true);

            Font = new SharedFontManager(device, fontStorage);

            VsyncEvent = new KEvent(KernelContext);

            DisplayResolutionChangeEvent = new KEvent(KernelContext);

            AccountManager = device.Configuration.AccountManager;
            ContentManager = device.Configuration.ContentManager;
            CaptureManager = new CaptureManager(device);

            // TODO: use set:sys (and get external clock source id from settings)
            // TODO: use "time!standard_steady_clock_rtc_update_interval_minutes" and implement a worker thread to be accurate.
            UInt128 clockSourceId = new UInt128(Guid.NewGuid().ToByteArray());
            IRtcManager.GetExternalRtcValue(out ulong rtcValue);

            // We assume the rtc is system time.
            TimeSpanType systemTime = TimeSpanType.FromSeconds((long)rtcValue);

            // Configure and setup internal offset
            TimeSpanType internalOffset = TimeSpanType.FromSeconds(device.Configuration.SystemTimeOffset);

            TimeSpanType systemTimeOffset = new TimeSpanType(systemTime.NanoSeconds + internalOffset.NanoSeconds);

            if (systemTime.IsDaylightSavingTime() && !systemTimeOffset.IsDaylightSavingTime())
            {
                internalOffset = internalOffset.AddSeconds(3600L);
            }
            else if (!systemTime.IsDaylightSavingTime() && systemTimeOffset.IsDaylightSavingTime())
            {
                internalOffset = internalOffset.AddSeconds(-3600L);
            }

            internalOffset = new TimeSpanType(-internalOffset.NanoSeconds);

            // First init the standard steady clock
            TimeServiceManager.Instance.SetupStandardSteadyClock(null, clockSourceId, systemTime, internalOffset, TimeSpanType.Zero, false);
            TimeServiceManager.Instance.SetupStandardLocalSystemClock(null, new SystemClockContext(), systemTime.ToSeconds());

            if (NxSettings.Settings.TryGetValue("time!standard_network_clock_sufficient_accuracy_minutes", out object standardNetworkClockSufficientAccuracyMinutes))
            {
                TimeSpanType standardNetworkClockSufficientAccuracy = new TimeSpanType((int)standardNetworkClockSufficientAccuracyMinutes * 60000000000);

                // The network system clock needs a valid system clock, as such we setup this system clock using the local system clock.
                TimeServiceManager.Instance.StandardLocalSystemClock.GetClockContext(null, out SystemClockContext localSytemClockContext);
                TimeServiceManager.Instance.SetupStandardNetworkSystemClock(localSytemClockContext, standardNetworkClockSufficientAccuracy);
            }

            TimeServiceManager.Instance.SetupStandardUserSystemClock(null, false, SteadyClockTimePoint.GetRandom());

            // FIXME: TimeZone shoud be init here but it's actually done in ContentManager

            TimeServiceManager.Instance.SetupEphemeralNetworkSystemClock();

            DatabaseImpl.Instance.InitializeDatabase(device);

            HostSyncpoint = new NvHostSyncpt(device);

            SurfaceFlinger = new SurfaceFlinger(device);

            InitLibHacHorizon();
            InitializeAudioRenderer();
        }

        private void InitializeAudioRenderer()
        {
            AudioManager = new AudioManager();
            AudioOutputManager = new AudioOutputManager();
            AudioInputManager = new AudioInputManager();
            AudioRendererManager = new AudioRendererManager();
            AudioDeviceSessionRegistry = new VirtualDeviceSessionRegistry();

            IWritableEvent[] audioOutputRegisterBufferEvents = new IWritableEvent[Constants.AudioOutSessionCountMax];

            for (int i = 0; i < audioOutputRegisterBufferEvents.Length; i++)
            {
                KEvent registerBufferEvent = new KEvent(KernelContext);

                audioOutputRegisterBufferEvents[i] = new AudioKernelEvent(registerBufferEvent);
            }

            AudioOutputManager.Initialize(Device.AudioDeviceDriver, audioOutputRegisterBufferEvents);

            IWritableEvent[] audioInputRegisterBufferEvents = new IWritableEvent[Constants.AudioInSessionCountMax];

            for (int i = 0; i < audioInputRegisterBufferEvents.Length; i++)
            {
                KEvent registerBufferEvent = new KEvent(KernelContext);

                audioInputRegisterBufferEvents[i] = new AudioKernelEvent(registerBufferEvent);
            }

            AudioInputManager.Initialize(Device.AudioDeviceDriver, audioInputRegisterBufferEvents);

            IWritableEvent[] systemEvents = new IWritableEvent[Constants.AudioRendererSessionCountMax];

            for (int i = 0; i < systemEvents.Length; i++)
            {
                KEvent systemEvent = new KEvent(KernelContext);

                systemEvents[i] = new AudioKernelEvent(systemEvent);
            }

            AudioManager.Initialize(Device.AudioDeviceDriver.GetUpdateRequiredEvent(), AudioOutputManager.Update, AudioInputManager.Update);

            AudioRendererManager.Initialize(systemEvents, Device.AudioDeviceDriver);

            AudioManager.Start();
        }

        public void InitializeServices()
        {
            // Initialize our pseudo-sysmodules

            Sm = new SmServer(this);
            Bsdsockets = new SocketsServer(this);
            Account = new AccountServer(this);
            Fs = new FsServer(this);
            LogManager = new LmServer(this);
            Ncm = new NcmServer(this);
            ProcessMana = new PmServer(this);
            Usb = new UsbServer(this);
            Settings = new SettingsServer(this);
            Bluetooth = new BluetoothServer(this);
            Btm = new BtmServer(this);
            Friends = new FriendsServer(this);
            Ptm = new PtmServer(this);
            Hid = new HidServer(this);
            Audio = new AudioServer(this);
            Wlan = new WlanServer(this);
            Ldn = new LdnServer(this);
            Nvservices = new NvServer(this);
            Pcv = new PcvServer(this);
            Nvnflinger = new NvnServer(this);
            Pcie = new PcieServer(this);
            Ns = new NsServer(this);
            Nfc = new NfcServer(this);
            Psc = new PscServer(this);
            Capsrv = new CapsServer(this);
            Am = new AmServer(this);
            Ssl = new SslServer(this);
            Nim = new NimServer(this);
            Spl = new SplServer(this);
            Erpt = new ErptServer(this);
            Pctl = new PctlServer(this);
            Npns = new NpnsServer(this);
            Eupld = new EupldServer(this);
            Glue = new GlueServer(this);
            Es = new EsServer(this);
            Fatal = new FatalServer(this);
            Grc = new GrcServer(this);
            Ro = new RoServer(this);
            Sdb = new SdbServer(this);
            Migration = new MigServer(this);
            Olsc = new OlscServer(this);
            Loader = new LoaderServer(this);
            Ngct = new NgctServer(this);
            Nifm = new NifmServer(this);
            Vi = new ViServer(this);
            Bcat = new BcatServer(this);
        }

        public void LoadKip(string kipPath)
        {
            using IStorage kipFile = new LocalStorage(kipPath, FileAccess.Read);

            ProgramLoader.LoadKip(KernelContext, new KipExecutable(kipFile));
        }

        private void InitLibHacHorizon()
        {
            LibHac.Horizon horizon = new LibHac.Horizon(null, Device.FileSystem.FsServer);

            horizon.CreateHorizonClient(out HorizonClient ryujinxClient).ThrowIfFailure();
            horizon.CreateHorizonClient(out HorizonClient bcatClient).ThrowIfFailure();

            ryujinxClient.Sm.RegisterService(new LibHacIReader(this), "arp:r").ThrowIfFailure();
            new LibHacBcatServer(bcatClient);

            LibHacHorizonServer = horizon;
            LibHacHorizonClient = ryujinxClient;
        }

        public void ChangeDockedModeState(bool newState)
        {
            if (newState != State.DockedMode)
            {
                State.DockedMode = newState;
                PerformanceState.PerformanceMode = State.DockedMode ? PerformanceMode.Boost : PerformanceMode.Default;

                AppletState.Messages.Enqueue(AppletMessage.OperationModeChanged);
                AppletState.Messages.Enqueue(AppletMessage.PerformanceModeChanged);
                AppletState.MessageEvent.ReadableEvent.Signal();

                SignalDisplayResolutionChange();

                Device.Configuration.RefreshInputConfig?.Invoke();
            }
        }

        public void ReturnFocus()
        {
            AppletState.SetFocus(true);
        }

        public void SimulateWakeUpMessage()
        {
            AppletState.Messages.Enqueue(AppletMessage.Resume);
            AppletState.MessageEvent.ReadableEvent.Signal();
        }

        public void ScanAmiibo(int nfpDeviceId, string amiiboId, bool useRandomUuid)
        {
            if (NfpDevices[nfpDeviceId].State == NfpDeviceState.SearchingForTag)
            {
                NfpDevices[nfpDeviceId].State         = NfpDeviceState.TagFound;
                NfpDevices[nfpDeviceId].AmiiboId      = amiiboId;
                NfpDevices[nfpDeviceId].UseRandomUuid = useRandomUuid;
            }
        }

        public bool SearchingForAmiibo(out int nfpDeviceId)
        {
            nfpDeviceId = default;

            for (int i = 0; i < NfpDevices.Count; i++)
            {
                if (NfpDevices[i].State == NfpDeviceState.SearchingForTag)
                {
                    nfpDeviceId = i;

                    return true;
                }
            }

            return false;
        }

        public void SignalDisplayResolutionChange()
        {
            DisplayResolutionChangeEvent.ReadableEvent.Signal();
        }

        public void SignalVsync()
        {
            VsyncEvent.ReadableEvent.Signal();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                _isDisposed = true;

                KProcess terminationProcess = new KProcess(KernelContext);
                KThread terminationThread = new KThread(KernelContext);

                terminationThread.Initialize(0, 0, 0, 3, 0, terminationProcess, ThreadType.Kernel, () =>
                {
                    // Force all threads to exit.
                    lock (KernelContext.Processes)
                    {
                        // Terminate application.
                        foreach (KProcess process in KernelContext.Processes.Values.Where(x => x.Flags.HasFlag(ProcessCreationFlags.IsApplication)))
                        {
                            process.Terminate();
                            process.DecrementReferenceCount();
                        }

                        // The application existed, now surface flinger can exit too.
                        SurfaceFlinger.Dispose();

                        // Terminate HLE services (must be done after the application is already terminated,
                        // otherwise the application will receive errors due to service termination).
                        foreach (KProcess process in KernelContext.Processes.Values.Where(x => !x.Flags.HasFlag(ProcessCreationFlags.IsApplication)))
                        {
                            process.Terminate();
                            process.DecrementReferenceCount();
                        }

                        KernelContext.Processes.Clear();
                    }

                    // Exit ourself now!
                    KernelStatic.GetCurrentThread().Exit();
                });

                terminationThread.Start();

                // Wait until the thread is actually started.
                while (terminationThread.HostThread.ThreadState == ThreadState.Unstarted)
                {
                    Thread.Sleep(10);
                }

                // Wait until the termination thread is done terminating all the other threads.
                terminationThread.HostThread.Join();

                // Destroy nvservices channels as KThread could be waiting on some user events.
                // This is safe as KThread that are likely to call ioctls are going to be terminated by the post handler hook on the SVC facade.
                INvDrvServices.Destroy();

                AudioManager.Dispose();
                AudioOutputManager.Dispose();
                AudioInputManager.Dispose();

                AudioRendererManager.Dispose();

                KernelContext.Dispose();
            }
        }
    }
}
