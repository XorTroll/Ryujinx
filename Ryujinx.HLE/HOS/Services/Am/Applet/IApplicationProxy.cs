using Ryujinx.HLE.HOS.Services.Am.Applet.ApplicationProxy;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    class IApplicationProxy : AppletProxyBase
    {
        public IApplicationProxy(AppletContext self) : base(AppletProxyType.Application, self) { }

        [CommandHipc(20)]
        // GetApplicationFunctions() -> object<nn::am::service::IApplicationFunctions>
        public ResultCode GetApplicationFunctions(ServiceCtx context)
        {
            MakeObject(context, new IApplicationFunctions(_self));

            return ResultCode.Success;
        }
    }
}