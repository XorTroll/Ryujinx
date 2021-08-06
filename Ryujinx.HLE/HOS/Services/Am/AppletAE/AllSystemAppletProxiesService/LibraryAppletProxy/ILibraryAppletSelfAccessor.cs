using Ryujinx.Common;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.LibraryAppletProxy
{
    class ILibraryAppletSelfAccessor : IpcService
    {
        private LibraryAppletContext _selfContext;

        public ILibraryAppletSelfAccessor(LibraryAppletContext selfContext)
        {
            _selfContext = selfContext;
        }

        [CommandHipc(0)]
        // PopInData() -> object<nn::am::service::IStorage>
        public ResultCode PopInData(ServiceCtx context)
        {
            if (_selfContext.TryPopInData(out var data, false))
            {
                MakeObject(context, new IStorage(data));

                return ResultCode.Success;
            }
            else
            {
                return ResultCode.NotAvailable;
            }
        }

        [CommandHipc(1)]
        // PushOutData(object<nn::am::service::IStorage>)
        public ResultCode PushOutData(ServiceCtx context)
        {
            var data = GetObject<IStorage>(context, 0);

            _selfContext.PushOutData(data.Data, false);

            return ResultCode.Success;
        }

        [CommandHipc(2)]
        // PopInteractiveInData() -> object<nn::am::service::IStorage>
        public ResultCode PopInteractiveInData(ServiceCtx context)
        {
            if (_selfContext.TryPopInData(out var data, true))
            {
                MakeObject(context, new IStorage(data));

                return ResultCode.Success;
            }
            else
            {
                return ResultCode.NotAvailable;
            }
        }

        [CommandHipc(3)]
        // PushInteractiveOutData(object<nn::am::service::IStorage>)
        public ResultCode PushInteractiveOutData(ServiceCtx context)
        {
            var data = GetObject<IStorage>(context, 0);

            _selfContext.PushOutData(data.Data, true);

            return ResultCode.Success;
        }

        [CommandHipc(10)]
        // ExitProcessAndReturn()
        public ResultCode ExitProcessAndReturn(ServiceCtx context)
        {
            _selfContext.Terminate();

            return ResultCode.Success;
        }

        [CommandHipc(11)]
        // GetLibraryAppletInfo() -> nn::am::service::LibraryAppletInfo
        public ResultCode GetLibraryAppletInfo(ServiceCtx context)
        {
            var libraryAppletInfo = new LibraryAppletInfo()
            {
                AppletId          = _selfContext.AppletId,
                LibraryAppletMode = _selfContext.LibraryAppletMode
            };

            context.ResponseData.WriteStruct(libraryAppletInfo);

            return ResultCode.Success;
        }

        [CommandHipc(14)]
        // GetCallerAppletIdentityInfo() -> nn::am::service::AppletIdentityInfo
        public ResultCode GetCallerAppletIdentityInfo(ServiceCtx context)
        {
            if (Horizon.Instance.AppletState.Applets.TryGetValue(_selfContext.CallerProcessId, out var callerApplet))
            {
                var appletIdentifyInfo = new AppletIdentifyInfo()
                {
                    AppletId = callerApplet.AppletId,
                    TitleId = AppletContext.GetProgramIdFromAppletId(callerApplet.AppletId, false)
                };

                context.ResponseData.WriteStruct(appletIdentifyInfo);

                return ResultCode.Success;
            }
            else
            {
                throw new System.Exception();
            }
        }
    }
}