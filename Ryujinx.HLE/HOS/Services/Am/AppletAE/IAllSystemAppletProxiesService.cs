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
            MakeObject(context, new ISystemAppletProxy(context.Request.HandleDesc.PId));

            return ResultCode.Success;
        }

        [CommandHipc(200)]
        [CommandHipc(201)] // 3.0.0+
        // OpenLibraryAppletProxy(u64, pid, handle<copy>) -> object<nn::am::service::ILibraryAppletProxy>
        public ResultCode OpenLibraryAppletProxy(ServiceCtx context)
        {
            MakeObject(context, new ILibraryAppletProxy(context.Request.HandleDesc.PId));

            return ResultCode.Success;
        }

        [CommandHipc(300)]
        // OpenOverlayAppletProxy(u64, pid, handle<copy>) -> object<nn::am::service::IOverlayAppletProxy>
        public ResultCode OpenOverlayAppletProxy(ServiceCtx context)
        {
            MakeObject(context, new IOverlayAppletProxy(context.Request.HandleDesc.PId));

            return ResultCode.Success;
        }

        [CommandHipc(350)]
        // OpenSystemApplicationProxy(u64, pid, handle<copy>) -> object<nn::am::service::IApplicationProxy>
        public ResultCode OpenSystemApplicationProxy(ServiceCtx context)
        {
            MakeObject(context, new IApplicationProxy(context.Request.HandleDesc.PId));

            return ResultCode.Success;
        }
    }
}