using Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    enum AppletProxyType
    {
        LibraryApplet,
        SystemApplet,
        OverlayApplet,
        Application
    }

    class AppletProxyBase : IpcService
    {
        private AppletProxyType _type;
        private long _pid;

        public AppletProxyBase(AppletProxyType type, long pid)
        {
            _type = type;
            _pid = pid;
        }

        [CommandHipc(0)]
        // GetCommonStateGetter() -> object<nn::am::service::ICommonStateGetter>
        public ResultCode GetCommonStateGetter(ServiceCtx context)
        {
            MakeObject(context, new ICommonStateGetter());

            return ResultCode.Success;
        }

        [CommandHipc(1)]
        // GetSelfController() -> object<nn::am::service::ISelfController>
        public ResultCode GetSelfController(ServiceCtx context)
        {
            MakeObject(context, new ISelfController(_pid));

            return ResultCode.Success;
        }

        [CommandHipc(2)]
        // GetWindowController() -> object<nn::am::service::IWindowController>
        public ResultCode GetWindowController(ServiceCtx context)
        {
            MakeObject(context, new IWindowController(_pid));

            return ResultCode.Success;
        }

        [CommandHipc(3)]
        // GetAudioController() -> object<nn::am::service::IAudioController>
        public ResultCode GetAudioController(ServiceCtx context)
        {
            MakeObject(context, new IAudioController());

            return ResultCode.Success;
        }

        [CommandHipc(4)]
        // GetDisplayController() -> object<nn::am::service::IDisplayController>
        public ResultCode GetDisplayController(ServiceCtx context)
        {
            MakeObject(context, new IDisplayController());

            return ResultCode.Success;
        }

        [CommandHipc(10)]
        // GetProcessWindingController() -> object<nn::am::service::IProcessWindingController>
        public ResultCode GetProcessWindingController(ServiceCtx context)
        {
            MakeObject(context, new IProcessWindingController());

            return ResultCode.Success;
        }

        [CommandHipc(11)]
        // GetLibraryAppletCreator() -> object<nn::am::service::ILibraryAppletCreator>
        public ResultCode GetLibraryAppletCreator(ServiceCtx context)
        {
            MakeObject(context, new ILibraryAppletCreator());

            return ResultCode.Success;
        }

        [CommandHipc(1000)]
        // GetDebugFunctions() -> object<nn::am::service::IDebugFunctions>
        public ResultCode GetDebugFunctions(ServiceCtx context)
        {
            MakeObject(context, new IDebugFunctions());

            return ResultCode.Success;
        }

        public ResultCode GetAppletCommonFunctionsImpl(ServiceCtx context)
        {
            MakeObject(context, new IAppletCommonFunctions());

            return ResultCode.Success;
        }
    }
}
