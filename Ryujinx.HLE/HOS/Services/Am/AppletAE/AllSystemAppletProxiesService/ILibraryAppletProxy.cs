using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.LibraryAppletProxy;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService
{
    class ILibraryAppletProxy : AppletProxyBase
    {
        public ILibraryAppletProxy(long pid) : base(AppletProxyType.LibraryApplet, pid) { }

        [CommandHipc(20)]
        // OpenLibraryAppletSelfAccessor() -> object<nn::am::service::ILibraryAppletSelfAccessor>
        public ResultCode OpenLibraryAppletSelfAccessor(ServiceCtx context)
        {
            MakeObject(context, new ILibraryAppletSelfAccessor());

            return ResultCode.Success;
        }

        [CommandHipc(21)]
        // GetAppletCommonFunctions() -> object<nn::am::service::IAppletCommonFunctions>
        public ResultCode GetAppletCommonFunctions(ServiceCtx context)
        {
            return GetAppletCommonFunctionsImpl(context);
        }
    }
}