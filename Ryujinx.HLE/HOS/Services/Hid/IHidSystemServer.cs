using Ryujinx.Common;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Services.Hid.Server;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;

namespace Ryujinx.HLE.HOS.Services.Hid
{
    [Service("hid:sys")]
    class IHidSystemServer : IpcService
    {
        private KEvent _joyDetachOnBluetoothOffEvent;
        private int _joyDetachOnBluetoothOffEventHandle;

        private KEvent _deviceRegisteredEventForControllerSupport;
        private int _deviceRegisteredEventForControllerSupportHandle;

        private KEvent _connectionTriggerTimeoutEvent;
        private int _connectionTriggerTimeoutEventHandle;

        public IHidSystemServer()
        {
            _joyDetachOnBluetoothOffEvent = new KEvent(Horizon.Instance.KernelContext);
            _deviceRegisteredEventForControllerSupport = new KEvent(Horizon.Instance.KernelContext);
            _connectionTriggerTimeoutEvent = new KEvent(Horizon.Instance.KernelContext);
        }

        [CommandHipc(303)]
        // ApplyNpadSystemCommonPolicy(u64)
        public ResultCode ApplyNpadSystemCommonPolicy(ServiceCtx context)
        {
            ulong commonPolicy = context.RequestData.ReadUInt64();

            Logger.Stub?.PrintStub(LogClass.ServiceHid, new { commonPolicy });

            return ResultCode.Success;
        }

        [CommandHipc(306)]
        // GetLastActiveNpad() -> u32
        public ResultCode GetLastActiveNpad(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(HidUtils.GetNpadIdTypeFromIndex(PlayerIndex.Player1));

            Logger.Stub?.PrintStub(LogClass.ServiceHid);

            return ResultCode.Success;
        }

        [CommandHipc(307)]
        // GetNpadSystemExtStyle() -> u64
        public ResultCode GetNpadSystemExtStyle(ServiceCtx context)
        {
            foreach (PlayerIndex playerIndex in Horizon.Instance.Device.Hid.Npads.GetSupportedPlayers())
            {
                if (HidUtils.GetNpadIdTypeFromIndex(playerIndex) > NpadIdType.Handheld)
                {
                    return ResultCode.InvalidNpadIdType;
                }
            }

            context.ResponseData.Write((ulong)Horizon.Instance.Device.Hid.Npads.SupportedStyleSets);

            return ResultCode.Success;
        }

        [CommandHipc(314)] // 9.0.0+
        // GetAppletFooterUiType(NpadIdType) -> u8
        public ResultCode GetAppletFooterUiType(ServiceCtx context)
        {
            ResultCode resultCode = GetAppletFooterUiTypeImpl(context, out AppletFooterUiType appletFooterUiType);

            context.ResponseData.WriteStruct(appletFooterUiType);

            return resultCode;
        }

        [CommandHipc(544)]
        // AcquireConnectionTriggerTimeoutEvent() -> handle<copy>
        public ResultCode AcquireConnectionTriggerTimeoutEvent(ServiceCtx context)
        {
            if (_connectionTriggerTimeoutEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_connectionTriggerTimeoutEvent.ReadableEvent, out _connectionTriggerTimeoutEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_connectionTriggerTimeoutEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceHid);

            return ResultCode.Success;
        }

        [CommandHipc(546)]
        // AcquireDeviceRegisteredEventForControllerSupport() -> handle<copy>
        public ResultCode AcquireDeviceRegisteredEventForControllerSupport(ServiceCtx context)
        {
            if (_deviceRegisteredEventForControllerSupportHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_deviceRegisteredEventForControllerSupport.ReadableEvent, out _deviceRegisteredEventForControllerSupportHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_deviceRegisteredEventForControllerSupportHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceHid);

            return ResultCode.Success;
        }

        [CommandHipc(703)]
        // GetUniquePadIds() -> u64, buffer
        public ResultCode GetUniquePadIds(ServiceCtx context)
        {
            // TODO: implement this accurately?

            var uniquePadIds = new ulong[] { 0x21, 0x22 };
            var uniquePadIdBuf = context.Request.RecvListBuff[0];
            for(var i = 0; i < uniquePadIds.Length; i++)
            {
                context.Memory.Write(uniquePadIdBuf.Position + (ulong)i * sizeof(ulong), uniquePadIds[i]);
            }

            context.ResponseData.Write((ulong)uniquePadIds.Length);

            Logger.Stub?.PrintStub(LogClass.Hid);

            return ResultCode.Success;
        }

        [CommandHipc(751)]
        // AcquireJoyDetachOnBluetoothOffEventHandle() -> handle<copy>
        public ResultCode AcquireJoyDetachOnBluetoothOffEventHandle(ServiceCtx context)
        {
            if (_joyDetachOnBluetoothOffEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_joyDetachOnBluetoothOffEvent.ReadableEvent, out _joyDetachOnBluetoothOffEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_joyDetachOnBluetoothOffEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceHid);

            return ResultCode.Success;
        }

        [CommandHipc(850)]
        // IsUsbFullKeyControllerEnabled() -> bool
        public ResultCode IsUsbFullKeyControllerEnabled(ServiceCtx context)
        {
            // TODO

            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServiceHid);

            return ResultCode.Success;
        }

        [CommandHipc(1000)]
        // InitializeFirmwareUpdate()
        public ResultCode InitializeFirmwareUpdate(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceHid);

            return ResultCode.Success;
        }

        [CommandHipc(1004)]
        // CheckFirmwareUpdateRequired(nn::hid::system::UniquePadId) -> u64?
        public ResultCode CheckFirmwareUpdateRequired(ServiceCtx context)
        {
            // TODO

            var uniquePadId = context.RequestData.ReadUInt64();

            context.ResponseData.Write((ulong)0);

            Logger.Stub?.PrintStub(LogClass.Hid, new { uniquePadId });

            return ResultCode.Success;
        }

        [CommandHipc(1153)]
        // GetTouchScreenDefaultConfiguration() -> ?
        public ResultCode GetTouchScreenDefaultConfiguration(ServiceCtx context)
        {
            // TODO
            Logger.Stub?.PrintStub(LogClass.ServiceHid);

            return ResultCode.Success;
        }

        private ResultCode GetAppletFooterUiTypeImpl(ServiceCtx context, out AppletFooterUiType appletFooterUiType)
        {
            NpadIdType  npadIdType  = (NpadIdType)context.RequestData.ReadUInt32();
            PlayerIndex playerIndex = HidUtils.GetIndexFromNpadIdType(npadIdType);

            // TODO: global value for this, which gets set in the shmems?

            appletFooterUiType = AppletFooterUiType.HandheldJoyConLeftJoyConRight;

            return ResultCode.Success;
        }
    }
}