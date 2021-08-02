using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.OverlayAppletProxy;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService
{
    class IOverlayAppletProxy : AppletProxyBase
    {
        public IOverlayAppletProxy(long pid) : base(AppletProxyType.OverlayApplet, pid) { }

        [CommandHipc(20)]
        // GetOverlayFunctions() -> object<nn::am::service::IOverlayFunctions>
        public ResultCode GetOverlayFunctions(ServiceCtx context)
        {
            MakeObject(context, new IOverlayFunctions());

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