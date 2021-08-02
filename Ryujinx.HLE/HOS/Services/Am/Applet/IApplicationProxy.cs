using Ryujinx.HLE.HOS.Services.Am.Applet.ApplicationProxy;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    class IApplicationProxy : AppletProxyBase
    {
        public IApplicationProxy(long pid) : base(AppletProxyType.Application, pid) { }

        [CommandHipc(20)]
        // GetApplicationFunctions() -> object<nn::am::service::IApplicationFunctions>
        public ResultCode GetApplicationFunctions(ServiceCtx context)
        {
            MakeObject(context, new IApplicationFunctions(context.Device.System));

            return ResultCode.Success;
        }
    }
}