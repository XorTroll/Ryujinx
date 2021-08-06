using Ryujinx.HLE.HOS.Services.Am.Applet;

namespace Ryujinx.HLE.HOS.Services.Am.AppletOE
{
    [Service("appletOE")]
    class IApplicationProxyService : IpcService
    {
        [CommandHipc(0)]
        // OpenApplicationProxy(u64, pid, handle<copy>) -> object<nn::am::service::IApplicationProxy>
        public ResultCode OpenApplicationProxy(ServiceCtx context)
        {
            if (Horizon.Instance.AppletState.Applets.TryGetValue(context.Request.HandleDesc.PId, out var self))
            {
                MakeObject(context, new IApplicationProxy(self));
                return ResultCode.Success;
            }
            else
            {
                return ResultCode.NotAvailable;
            }
        }
    }
}