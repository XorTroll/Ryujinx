using Ryujinx.Common;
using Ryujinx.HLE.HOS.Ipc;
using System;
using Ryujinx.HLE.HOS.Services.Am.Applet;
using Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy;

namespace Ryujinx.HLE.HOS.Services.Am.AppletAE.AllSystemAppletProxiesService.LibraryAppletProxy
{
    class ILibraryAppletSelfAccessor : IpcService
    {
        private LibraryAppletContext _selfContext;
        private int _interactiveInDataEventHandle;

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

        [CommandHipc(6)]
        // GetPopInteractiveInDataEvent() -> handle<copy>
        public ResultCode GetPopInteractiveInDataEvent(ServiceCtx context)
        {
            if (_interactiveInDataEventHandle == 0)
            {
                if (!_selfContext.TryCreatePopInteractiveInDataEventHandle(context.Process, out _interactiveInDataEventHandle))
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_interactiveInDataEventHandle);

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

        [CommandHipc(12)]
        // GetMainAppletIdentityInfo() -> nn::am::service::AppletIdentityInfo
        public ResultCode GetMainAppletIdentityInfo(ServiceCtx context)
        {
            // TODO: is it always qlaunch?

            var appletIdentifyInfo = new AppletIdentifyInfo()
            {
                AppletId = AppletId.SystemAppletMenu,
                ApplicationId = 0x0 // Only used for applications (hence it's name)
            };

            context.ResponseData.WriteStruct(appletIdentifyInfo);

            return ResultCode.Success;
        }

        [CommandHipc(13)]
        // CanUseApplicationCore() -> bool
        public ResultCode CanUseApplicationCore(ServiceCtx context)
        {
            context.ResponseData.Write(true);

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
                    ApplicationId = 0x0 // TODO: if callerApplet.AppletId == AppletId.Application, get the running app's ID
                };

                context.ResponseData.WriteStruct(appletIdentifyInfo);

                return ResultCode.Success;
            }
            else
            {
                throw new System.Exception();
            }
        }

        [CommandHipc(19)]
        // GetDesirableKeyboardLayout() -> nn::settings::KeyboardLayout
        public ResultCode GetDesirableKeyboardLayout(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.DesiredKeyboardLayout);

            return ResultCode.Success;
        }

        [CommandHipc(30)]
        // UnpopInData(object<nn::am::service::IStorage>)
        public ResultCode UnpopInData(ServiceCtx context)
        {
            var data = GetObject<IStorage>(context, 0);

            _selfContext.PushInData(data.Data, false);

            return ResultCode.Success;
        }

        [CommandHipc(60)]
        // GetMainAppletApplicationDesiredLanguage() -> s64
        public ResultCode GetMainAppletApplicationDesiredLanguage(ServiceCtx context)
        {
            context.ResponseData.Write(Horizon.Instance.State.DesiredLanguageCode);

            return ResultCode.Success;
        }
    }
}