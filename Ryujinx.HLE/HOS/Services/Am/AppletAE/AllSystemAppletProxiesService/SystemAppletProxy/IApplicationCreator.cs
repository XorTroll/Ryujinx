using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.SystemAppletProxy.ApplicationCreator;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.SystemAppletProxy
{
    class IApplicationCreator : IpcService
    {
        private AppletContext _self;

        public IApplicationCreator(AppletContext self)
        {
            _self = self;
        }

        [CommandHipc(0)]
        // CreateApplication(nn::ncm::ApplicationId) -> object<nn::am::service::IApplicationAccessor>
        public ResultCode CreateApplication(ServiceCtx context)
        {
            var applicationId = context.RequestData.ReadUInt64();

            MakeObject(context, new IApplicationAccessor(_self, applicationId, false));

            return ResultCode.Success;
        }
        
        [CommandHipc(10)]
        // CreateSystemApplication(nn::ncm::SystemApplicationId) -> object<nn::am::service::IApplicationAccessor>
        public ResultCode CreateSystemApplication(ServiceCtx context)
        {
            var systemApplicationId = context.RequestData.ReadUInt64();

            MakeObject(context, new IApplicationAccessor(_self, systemApplicationId, true));

            return ResultCode.Success;
        }
    }
}