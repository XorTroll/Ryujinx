using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE
{
    [Service("appletAE")]
    class IAllSystemAppletProxiesService : IpcService
    {
        [CommandHipc(100)]
        // OpenSystemAppletProxy(u64, pid, handle<copy>) -> object<nn::am::service::ISystemAppletProxy>
        public ResultCode OpenSystemAppletProxy(ServiceCtx context)
        {
            if (Horizon.Instance.AppletState.Applets.TryGetValue(context.Request.HandleDesc.PId, out var self) && self.IsSystemApplet)
            {
                MakeObject(context, new ISystemAppletProxy(self));
                return ResultCode.Success;
            }
            else
            {
                return ResultCode.NotAvailable;
            }
        }

        [CommandHipc(200)]
        [CommandHipc(201)] // 3.0.0+
        // OpenLibraryAppletProxy(u64, pid, handle<copy>) -> object<nn::am::service::ILibraryAppletProxy>
        public ResultCode OpenLibraryAppletProxy(ServiceCtx context)
        {
            if (Horizon.Instance.AppletState.Applets.TryGetValue(context.Request.HandleDesc.PId, out var self) && self.IsLibraryApplet)
            {
                MakeObject(context, new ILibraryAppletProxy(self));
                return ResultCode.Success;
            }
            else
            {
                return ResultCode.NotAvailable;
            }
        }

        [CommandHipc(300)]
        // OpenOverlayAppletProxy(u64, pid, handle<copy>) -> object<nn::am::service::IOverlayAppletProxy>
        public ResultCode OpenOverlayAppletProxy(ServiceCtx context)
        {
            if (Horizon.Instance.AppletState.Applets.TryGetValue(context.Request.HandleDesc.PId, out var self) && self.IsOverlayApplet)
            {
                MakeObject(context, new IOverlayAppletProxy(self));
                return ResultCode.Success;
            }
            else
            {
                return ResultCode.NotAvailable;
            }
        }

        [CommandHipc(350)]
        // OpenSystemApplicationProxy(u64, pid, handle<copy>) -> object<nn::am::service::IApplicationProxy>
        public ResultCode OpenSystemApplicationProxy(ServiceCtx context)
        {
            if (Horizon.Instance.AppletState.Applets.TryGetValue(context.Request.HandleDesc.PId, out var self) && self.IsSystemApplication)
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