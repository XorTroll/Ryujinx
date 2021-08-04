using Ryujinx.Common;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services.Ptm.Apm;
using Ryujinx.HLE.HOS.Services.Ptm.Lbl;
using System;

namespace Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy
{
    class ICommonStateGetter : IpcService
    {
        private ManagerServer       _apmManagerServer;
        private SystemManagerServer _apmSystemManagerServer;
        private LblControllerServer _lblControllerServer;

        private bool _vrModeEnabled;
#pragma warning disable CS0414
        private bool _lcdBacklighOffEnabled;
        private bool _requestExitToLibraryAppletAtExecuteNextProgramEnabled;
#pragma warning restore CS0414
        private int  _messageEventHandle;
        private int  _displayResolutionChangedEventHandle;

        private KEvent _acquiredSleepLockEvent;
        private int _acquiredSleepLockEventHandle;

        public ICommonStateGetter()
        {
            _apmManagerServer       = new ManagerServer();
            _apmSystemManagerServer = new SystemManagerServer();
            _lblControllerServer    = new LblControllerServer();
            _acquiredSleepLockEvent = new KEvent(Horizon.Instance.KernelContext);
        }

        [CommandHipc(0)]
        // GetEventHandle() -> handle<copy>
        public ResultCode GetEventHandle(ServiceCtx context)
        {
            KEvent messageEvent = Horizon.Instance.AppletState.MessageEvent;

            if (_messageEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(messageEvent.ReadableEvent, out _messageEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_messageEventHandle);

            return ResultCode.Success;
        }

        [CommandHipc(1)]
        // ReceiveMessage() -> nn::am::AppletMessage
        public ResultCode ReceiveMessage(ServiceCtx context)
        {
            if (!Horizon.Instance.AppletState.Messages.TryDequeue(out AppletMessage message))
            {
                return ResultCode.NoMessages;
            }

            KEvent messageEvent = Horizon.Instance.AppletState.MessageEvent;

            // NOTE: Service checks if current states are different than the stored ones.
            //       Since we don't support any states for now, it's fine to check if there is still messages available.

            if (Horizon.Instance.AppletState.Messages.IsEmpty)
            {
                messageEvent.ReadableEvent.Clear();
            }
            else
            {
                messageEvent.ReadableEvent.Signal();
            }

            context.ResponseData.Write((int)message);

            return ResultCode.Success;
        }

        [CommandHipc(5)]
        // GetOperationMode() -> u8
        public ResultCode GetOperationMode(ServiceCtx context)
        {
            OperationMode mode = Horizon.Instance.State.DockedMode
                ? OperationMode.Docked
                : OperationMode.Handheld;

            context.ResponseData.Write((byte)mode);

            return ResultCode.Success;
        }

        [CommandHipc(6)]
        // GetPerformanceMode() -> nn::apm::PerformanceMode
        public ResultCode GetPerformanceMode(ServiceCtx context)
        {
            return (ResultCode)_apmManagerServer.GetPerformanceMode(context);
        }

        [CommandHipc(8)]
        // GetBootMode() -> u8
        public ResultCode GetBootMode(ServiceCtx context)
        {
            context.ResponseData.Write((byte)0); //Unknown value.

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(9)]
        // GetCurrentFocusState() -> u8
        public ResultCode GetCurrentFocusState(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.AppletState.FocusState);

            return ResultCode.Success;
        }

        [CommandHipc(10)]
        // RequestToAcquireSleepLock()
        public ResultCode RequestToAcquireSleepLock(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(13)]
        // GetAcquiredSleepLockEvent() -> handle<copy>
        public ResultCode GetAcquiredSleepLockEvent(ServiceCtx context)
        {
            if (_acquiredSleepLockEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_acquiredSleepLockEvent.ReadableEvent, out _acquiredSleepLockEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_acquiredSleepLockEventHandle);
            // (for qlaunch) insta-signal the event so that qlaunch knows it has acquired sleep lock
            _acquiredSleepLockEvent.WritableEvent.Signal();

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(20)]
        // PushToGeneralChannel(object<nn::am::service::IStorage>)
        public ResultCode PushToGeneralChannel(ServiceCtx context)
        {
            // TODO

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(31)]
        // GetReaderLockAccessorEx(u32) -> object<nn::am::service::ILockAccessor>
        public ResultCode GetReaderLockAccessorEx(ServiceCtx context)
        {
            var unk = context.RequestData.ReadUInt32();

            Logger.Stub?.PrintStub(LogClass.ServiceAm, new { unk });

            MakeObject(context, new ILockAccessor(unk));

            return ResultCode.Success;
        }

        [CommandHipc(50)] // 3.0.0+
        // IsVrModeEnabled() -> b8
        public ResultCode IsVrModeEnabled(ServiceCtx context)
        {
            context.ResponseData.Write(_vrModeEnabled);

            return ResultCode.Success;
        }

        [CommandHipc(51)] // 3.0.0+
        // SetVrModeEnabled(b8)
        public ResultCode SetVrModeEnabled(ServiceCtx context)
        {
            bool vrModeEnabled = context.RequestData.ReadBoolean();

            UpdateVrMode(vrModeEnabled);

            return ResultCode.Success;
        }

        [CommandHipc(52)] // 4.0.0+
        // SetLcdBacklighOffEnabled(b8)
        public ResultCode SetLcdBacklighOffEnabled(ServiceCtx context)
        {
            // NOTE: Service sets a private field here, maybe this field is used somewhere else to turned off the backlight.
            //       Since we don't support backlight, it's fine to do nothing.

            _lcdBacklighOffEnabled = context.RequestData.ReadBoolean();

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(53)] // 7.0.0+
        // BeginVrModeEx()
        public ResultCode BeginVrModeEx(ServiceCtx context)
        {
            UpdateVrMode(true);

            return ResultCode.Success;
        }

        [CommandHipc(54)] // 7.0.0+
        // EndVrModeEx()
        public ResultCode EndVrModeEx(ServiceCtx context)
        {
            UpdateVrMode(false);

            return ResultCode.Success;
        }

        private void UpdateVrMode(bool vrModeEnabled)
        {
            if (_vrModeEnabled == vrModeEnabled)
            {
                return;
            }

            _vrModeEnabled = vrModeEnabled;

            if (vrModeEnabled)
            {
                _lblControllerServer.EnableVrMode();
            }
            else
            {
                _lblControllerServer.DisableVrMode();
            }

            // TODO: It signals an internal event of ICommonStateGetter. We have to determine where this event is used.
        }

        [CommandHipc(60)] // 3.0.0+
        // GetDefaultDisplayResolution() -> (u32, u32)
        public ResultCode GetDefaultDisplayResolution(ServiceCtx context)
        {
            context.ResponseData.Write(1280);
            context.ResponseData.Write(720);

            return ResultCode.Success;
        }

        [CommandHipc(61)] // 3.0.0+
        // GetDefaultDisplayResolutionChangeEvent() -> handle<copy>
        public ResultCode GetDefaultDisplayResolutionChangeEvent(ServiceCtx context)
        {
            if (_displayResolutionChangedEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(Horizon.Instance.DisplayResolutionChangeEvent.ReadableEvent, out _displayResolutionChangedEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_displayResolutionChangedEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(66)] // 6.0.0+
        // SetCpuBoostMode(u32 cpu_boost_mode)
        public ResultCode SetCpuBoostMode(ServiceCtx context)
        {
            uint cpuBoostMode = context.RequestData.ReadUInt32();

            if (cpuBoostMode > 1)
            {
                return ResultCode.InvalidParameters;
            }

            _apmSystemManagerServer.SetCpuBoostMode((CpuBoostMode)cpuBoostMode);

            // TODO: It signals an internal event of ICommonStateGetter. We have to determine where this event is used.

            return ResultCode.Success;
        }

        [CommandHipc(91)] // 7.0.0+
        // GetCurrentPerformanceConfiguration() -> nn::apm::PerformanceConfiguration
        public ResultCode GetCurrentPerformanceConfiguration(ServiceCtx context)
        {
            return (ResultCode)_apmSystemManagerServer.GetCurrentPerformanceConfiguration(context);
        }

        [CommandHipc(200)] // 7.0.0+
        // GetOperationModeSystemInfo() -> u32
        public ResultCode GetOperationModeSystemInfo(ServiceCtx context)
        {
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            return ResultCode.Success;
        }

        [CommandHipc(300)] // 9.0.0+
        // GetSettingsPlatformRegion() -> u8
        public ResultCode GetSettingsPlatformRegion(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.PlatformRegion);

            return ResultCode.Success;
        }

        [CommandHipc(900)] // 11.0.0+
        // SetRequestExitToLibraryAppletAtExecuteNextProgramEnabled()
        public ResultCode SetRequestExitToLibraryAppletAtExecuteNextProgramEnabled(ServiceCtx context)
        {
            // TODO : Find where the field is used.
            _requestExitToLibraryAppletAtExecuteNextProgramEnabled = true;

            return ResultCode.Success;
        }
    }
}