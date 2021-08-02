using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;

namespace Ryujinx.HLE.HOS.Services.Ns
{
    [Service("ns:am")]
    class IApplicationManagerInterface : IpcService
    {
        private KEvent _applicationRecordUpdateEvent;
        private int _applicationRecordUpdateEventHandle;

        private KEvent _gameCardUpdateDetectionEvent;
        private int _gameCardUpdateDetectionEventHandle;

        private KEvent _sdCardMountStatusChangedEvent;
        private int _sdCardMountStatusChangedEventHandle;

        private KEvent _gameCardMountFailureEvent;
        private int _gameCardMountFailureEventHandle;

        public IApplicationManagerInterface(KernelContext context)
        {
            _applicationRecordUpdateEvent = new KEvent(context);
            _gameCardUpdateDetectionEvent = new KEvent(context);
            _sdCardMountStatusChangedEvent = new KEvent(context);
            _gameCardMountFailureEvent = new KEvent(context);
        }

        [CommandHipc(0)]
        // ListApplicationRecord() -> u32, buffer?
        public ResultCode ListApplicationRecord(ServiceCtx context)
        {
            // TODO: installed applications, etc.
            // For now, let's say there are no applications
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.ServiceNs);

            return ResultCode.Success;
        }

        [CommandHipc(2)]
        // GetApplicationRecordUpdateSystemEvent() -> handle<copy>
        public ResultCode GetApplicationRecordUpdateSystemEvent(ServiceCtx context)
        {
            if (_applicationRecordUpdateEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_applicationRecordUpdateEvent.ReadableEvent, out _applicationRecordUpdateEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_applicationRecordUpdateEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceNs);

            return ResultCode.Success;
        }

        [CommandHipc(44)]
        // GetSdCardMountStatusChangedEvent() -> handle<copy>
        public ResultCode GetSdCardMountStatusChangedEvent(ServiceCtx context)
        {
            if (_sdCardMountStatusChangedEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_sdCardMountStatusChangedEvent.ReadableEvent, out _sdCardMountStatusChangedEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_sdCardMountStatusChangedEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceNs);

            return ResultCode.Success;
        }

        [CommandHipc(52)]
        // GetGameCardUpdateDetectionEvent() -> handle<copy>
        public ResultCode GetGameCardUpdateDetectionEvent(ServiceCtx context)
        {
            if (_gameCardUpdateDetectionEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_gameCardUpdateDetectionEvent.ReadableEvent, out _gameCardUpdateDetectionEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_gameCardUpdateDetectionEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceNs);

            return ResultCode.Success;
        }

        [CommandHipc(70)]
        // ResumeAll()
        public ResultCode ResumeAll(ServiceCtx context)
        {
            // TODO

            Logger.Stub?.PrintStub(LogClass.ServiceNs);

            return ResultCode.Success;
        }

        [CommandHipc(400)]
        // GetApplicationControlData(u8, u64) -> (unknown<4>, buffer<unknown, 6>)
        public ResultCode GetApplicationControlData(ServiceCtx context)
        {
            byte  source  = (byte)context.RequestData.ReadInt64();
            ulong titleId = context.RequestData.ReadUInt64();

            ulong position = context.Request.ReceiveBuff[0].Position;

            byte[] nacpData = context.Device.Application.ControlData.ByteSpan.ToArray();

            context.Memory.Write(position, nacpData);

            return ResultCode.Success;
        }

        [CommandHipc(505)]
        // GetGameCardMountFailureEvent() -> handle<copy>
        public ResultCode GetGameCardMountFailureEvent(ServiceCtx context)
        {
            if (_gameCardMountFailureEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_gameCardMountFailureEvent.ReadableEvent, out _gameCardMountFailureEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_gameCardMountFailureEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServiceNs);

            return ResultCode.Success;
        }
    }
}