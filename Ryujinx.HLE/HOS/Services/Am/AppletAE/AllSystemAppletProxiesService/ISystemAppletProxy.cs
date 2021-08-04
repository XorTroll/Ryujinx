using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.SystemAppletProxy;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService
{
    class ISystemAppletProxy : AppletProxyBase
    {
        public ISystemAppletProxy(long pid) : base(AppletProxyType.SystemApplet, pid) { }

        [CommandHipc(20)]
        // GetHomeMenuFunctions() -> object<nn::am::service::IHomeMenuFunctions>
        public ResultCode GetHomeMenuFunctions(ServiceCtx context)
        {
            MakeObject(context, new IHomeMenuFunctions());

            return ResultCode.Success;
        }

        [CommandHipc(21)]
        // GetGlobalStateController() -> object<nn::am::service::IGlobalStateController>
        public ResultCode GetGlobalStateController(ServiceCtx context)
        {
            MakeObject(context, new IGlobalStateController());

            return ResultCode.Success;
        }

        [CommandHipc(22)]
        // GetApplicationCreator() -> object<nn::am::service::IApplicationCreator>
        public ResultCode GetApplicationCreator(ServiceCtx context)
        {
            MakeObject(context, new IApplicationCreator());

            return ResultCode.Success;
        }

        [CommandHipc(23)]
        // GetAppletCommonFunctions() -> object<nn::am::service::IAppletCommonFunctions>
        public ResultCode GetAppletCommonFunctions(ServiceCtx context)
        {
            return GetAppletCommonFunctionsImpl(context);
        }
    }
}