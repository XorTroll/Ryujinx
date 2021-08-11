using Ryujinx.Common;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.SystemAppletProxy.ApplicationCreator
{
    class IApplicationAccessor : IAccessorBase
    {
        private ApplicationContext _context => _contextBase as ApplicationContext;

        public IApplicationAccessor(AppletContext self, ulong applicationId, bool isSystem) : base(self, new ApplicationContext(self.ProcessId, applicationId, isSystem))
        {
            Logger.Info?.Print(LogClass.ServiceAm, (isSystem ? "System application" : "Application") + $" 0x{applicationId:X16} created...");
        }

        // NOTE: Commands 0, 1, 10, 20, 25 and 30 are implemented in IAccessorBase

        [CommandHipc(101)]
        // RequestForApplicationToGetForeground()
        public ResultCode RequestForApplicationToGetForeground(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(110)]
        // TerminateAllLibraryApplets()
        public ResultCode TerminateAllLibraryApplets(ServiceCtx context)
        {
            // TODO: terminate all libapplets whose caller is this application

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(111)]
        // AreAnyLibraryAppletsLeft() -> bool
        public ResultCode AreAnyLibraryAppletsLeft(ServiceCtx context)
        {
            // TODO: count all libapplets whose caller is this application

            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(112)]
        // GetCurrentLibraryApplet() -> object<nn::am::service::IAppletAccessor>
        public ResultCode GetCurrentLibraryApplet(ServiceCtx context)
        {
            // TODO: be able to detect the current applet under this app

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.NotAvailable;
        }

        [CommandHipc(120)]
        // GetApplicationId() -> nn::ncm::ApplicationId
        public ResultCode GetApplicationId(ServiceCtx context)
        {
            context.ResponseData.Write(_context.ApplicationId);

            return ResultCode.Success;
        }

        [CommandHipc(121)]
        // PushLaunchParameter(u32, object<nn::am::service::IStorage>)
        public ResultCode PushLaunchParameter(ServiceCtx context)
        {
            var kind = context.RequestData.ReadStruct<LaunchParameterKind>();

            var data = GetObject<IStorage>(context, 0);

            _context.PushLaunchParameter(kind, data.Data);

            return ResultCode.Success;
        }

        [CommandHipc(122)]
        // GetApplicationControlProperty() -> buffer<bytes, 6>
        public ResultCode GetApplicationControlProperty(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.NotAvailable;
        }

        [CommandHipc(123)]
        // GetApplicationLaunchProperty() -> buffer<bytes, 6>
        public ResultCode GetApplicationLaunchProperty(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.NotAvailable;
        }
    }
}