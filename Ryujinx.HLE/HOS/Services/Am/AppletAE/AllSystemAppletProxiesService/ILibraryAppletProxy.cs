using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.LibraryAppletProxy;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService
{
    class ILibraryAppletProxy : AppletProxyBase
    {
        public ILibraryAppletProxy(AppletContext self) : base(AppletProxyType.LibraryApplet, self) { }

        [CommandHipc(20)]
        // OpenLibraryAppletSelfAccessor() -> object<nn::am::service::ILibraryAppletSelfAccessor>
        public ResultCode OpenLibraryAppletSelfAccessor(ServiceCtx context)
        {
            MakeObject(context, new ILibraryAppletSelfAccessor(_self.LibraryAppletContext));

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