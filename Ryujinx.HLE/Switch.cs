using Ryujinx.Audio.Backends.CompatLayer;
using Ryujinx.Audio.Integration;
using Ryujinx.Graphics.Gpu;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.FileSystem.Content;
using Ryujinx.HLE.HOS;
using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Ptm.Apm;
using Ryujinx.HLE.HOS.Services.Hid;
using Ryujinx.Memory;
using System;

namespace Ryujinx.HLE
{
    public class Switch : IDisposable
    {
        public HLEConfiguration Configuration { get; }

        public IHardwareDeviceDriver AudioDeviceDriver { get; }

        internal MemoryBlock Memory { get; }

        public GpuContext Gpu { get; }

        public VirtualFileSystem FileSystem => Configuration.VirtualFileSystem;

        public Horizon System { get; }

        public ApplicationLoader Application { get; }

        public PerformanceStatistics Statistics { get; }

        public Hid Hid { get; }

        public TamperMachine TamperMachine { get; }

        public IHostUiHandler UiHandler { get; }

        public bool EnableDeviceVsync { get; set; } = true;

        public Switch(HLEConfiguration configuration)
        {
            if (configuration.GpuRenderer == null)
            {
                throw new ArgumentNullException(nameof(configuration.GpuRenderer));
            }

            if (configuration.AudioDeviceDriver == null)
            {
                throw new ArgumentNullException(nameof(configuration.AudioDeviceDriver));
            }

            if (configuration.UserChannelPersistence== null)
            {
                throw new ArgumentNullException(nameof(configuration.UserChannelPersistence));
            }

            Configuration = configuration;

            UiHandler = configuration.HostUiHandler;

            AudioDeviceDriver = new CompatLayerHardwareDeviceDriver(configuration.AudioDeviceDriver);

            Memory = new MemoryBlock(configuration.MemoryConfiguration.ToDramSize(), MemoryAllocationFlags.Reserve);

            Gpu = new GpuContext(configuration.GpuRenderer);

            System = new Horizon(this);

            Statistics = new PerformanceStatistics();

            Hid = new Hid();

            Application = new ApplicationLoader(this);

            TamperMachine = new TamperMachine();
        }

        public void Initialize()
        {
            System.InitializeServices();

            Hid.InitDevices();

            System.State.SetLanguage(Configuration.SystemLanguage);

            System.State.SetRegion(Configuration.Region);

            EnableDeviceVsync = Configuration.EnableVsync;

            System.State.DockedMode = Configuration.EnableDockedMode;

            System.PerformanceState.PerformanceMode = System.State.DockedMode ? PerformanceMode.Boost : PerformanceMode.Default;

            System.EnablePtc = Configuration.EnablePtc;

            System.FsIntegrityCheckLevel = Configuration.FsIntegrityCheckLevel;

            System.GlobalAccessLogMode = Configuration.FsGlobalAccessLogMode;
        }

        public bool WaitFifo()
        {
            return Gpu.GPFifo.WaitForCommands();
        }

        public void ProcessFrame()
        {
            Gpu.Renderer.PreFrame();

            Gpu.GPFifo.DispatchCalls();
        }

        public bool ConsumeFrameAvailable()
        {
            return Gpu.Window.ConsumeFrameAvailable();
        }

        public void PresentFrame(Action swapBuffersCallback)
        {
            Gpu.Window.Present(swapBuffersCallback);
        }

        public void DisposeGpu()
        {
            Gpu.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                System.Dispose();
                AudioDeviceDriver.Dispose();
                FileSystem.Unload();
                Memory.Dispose();
            }
        }
    }
}
