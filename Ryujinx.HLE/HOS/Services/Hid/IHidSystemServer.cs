using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Services.Hid.Server;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel;
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

        public IHidSystemServer(KernelContext context)
        {
            _joyDetachOnBluetoothOffEvent = new KEvent(context);
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
        // GetLastActiveNpad(u32) -> u8, u8
        public ResultCode GetLastActiveNpad(ServiceCtx context)
        {
            // TODO: RequestData seems to have garbage data, reading an extra uint seems to fix the issue.
            context.RequestData.ReadUInt32();

            ResultCode resultCode = GetAppletFooterUiTypeImpl(context, out AppletFooterUiType appletFooterUiType);

            context.ResponseData.Write((byte)appletFooterUiType);
            context.ResponseData.Write((byte)0);

            return resultCode;
        }

        [CommandHipc(307)]
        // GetNpadSystemExtStyle() -> u64
        public ResultCode GetNpadSystemExtStyle(ServiceCtx context)
        {
            foreach (PlayerIndex playerIndex in context.Device.Hid.Npads.GetSupportedPlayers())
            {
                if (HidUtils.GetNpadIdTypeFromIndex(playerIndex) > NpadIdType.Handheld)
                {
                    return ResultCode.InvalidNpadIdType;
                }
            }

            context.ResponseData.Write((ulong)context.Device.Hid.Npads.SupportedStyleSets);

            return ResultCode.Success;
        }

        [CommandHipc(314)] // 9.0.0+
        // GetAppletFooterUiType(u32) -> u8
        public ResultCode GetAppletFooterUiType(ServiceCtx context)
        {
            ResultCode resultCode = GetAppletFooterUiTypeImpl(context, out AppletFooterUiType appletFooterUiType);

            context.ResponseData.Write((byte)appletFooterUiType);

            return resultCode;
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

            appletFooterUiType = context.Device.Hid.SharedMemory.Npads[(int)playerIndex].InternalState.AppletFooterUiType;

            return ResultCode.Success;
        }
    }
}