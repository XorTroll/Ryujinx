using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Memory;
using System;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy
{
    class IDisplayController : IpcService
    {
        private KTransferMemory _transferMem;
        private bool            _lastApplicationCaptureBufferAcquired;
        private bool            _callerAppletCaptureBufferAcquired;

        public IDisplayController()
        {
            _transferMem = Horizon.Instance.AppletCaptureBufferTransfer;
        }

        private byte[] UpdateBuffer()
        {
            // Test

            var buf = new byte[0x384000];
            var colorBuf = MemoryMarshal.Cast<byte, uint>(buf);
            for (var i = 0; i < colorBuf.Length; i++)
            {
                colorBuf[i] = 0xFF00FF00; // ARGB
            }

            return buf;
        }

        [CommandHipc(7)]
        // GetCallerAppletCaptureImageEx() -> (b8, buffer<bytes, 6>)
        public ResultCode GetCallerAppletCaptureImageEx(ServiceCtx context)
        {
            var imageBuf = context.Request.ReceiveBuff[0];

            Logger.Stub?.PrintStub(LogClass.ServiceAm);

            var buf = UpdateBuffer();
            context.Memory.Write(imageBuf.Position, buf);

            context.ResponseData.Write(true);

            return ResultCode.Success;
        }

        [CommandHipc(8)] // 2.0.0+
        // TakeScreenShotOfOwnLayer(b8, s32)
        public ResultCode TakeScreenShotOfOwnLayer(ServiceCtx context)
        {
            bool unknown1 = context.RequestData.ReadBoolean();
            int  unknown2 = context.RequestData.ReadInt32();

            Logger.Stub?.PrintStub(LogClass.ServiceAm, new { unknown1, unknown2 });

            return ResultCode.Success;
        }

        [CommandHipc(11)]
        // ReleaseLastApplicationCaptureBuffer()
        public ResultCode ReleaseLastApplicationCaptureBuffer(ServiceCtx context)
        {
            if (!_lastApplicationCaptureBufferAcquired)
            {
                return ResultCode.BufferNotAcquired;
            }

            _lastApplicationCaptureBufferAcquired = false;

            return ResultCode.Success;
        }

        [CommandHipc(15)]
        // ReleaseCallerAppletCaptureBuffer()
        public ResultCode ReleaseCallerAppletCaptureBuffer(ServiceCtx context)
        {
            if (!_callerAppletCaptureBufferAcquired)
            {
                return ResultCode.BufferNotAcquired;
            }

            _callerAppletCaptureBufferAcquired = false;

            return ResultCode.Success;
        }

        [CommandHipc(16)]
        // AcquireLastApplicationCaptureBufferEx() -> (b8, handle<copy>)
        public ResultCode AcquireLastApplicationCaptureBufferEx(ServiceCtx context)
        {
            if (_lastApplicationCaptureBufferAcquired)
            {
                return ResultCode.BufferAlreadyAcquired;
            }

            if (context.Process.HandleTable.GenerateHandle(_transferMem, out int handle) != KernelResult.Success)
            {
                throw new InvalidOperationException("Out of handles!");
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(handle);

            _lastApplicationCaptureBufferAcquired = true;

            context.ResponseData.Write(_lastApplicationCaptureBufferAcquired);

            return ResultCode.Success;
        }

        [CommandHipc(18)]
        // AcquireCallerAppletCaptureBufferEx() -> (b8, handle<copy>)
        public ResultCode AcquireCallerAppletCaptureBufferEx(ServiceCtx context)
        {
            if (_callerAppletCaptureBufferAcquired)
            {
                return ResultCode.BufferAlreadyAcquired;
            }

            if (context.Process.HandleTable.GenerateHandle(_transferMem, out int handle) != KernelResult.Success)
            {
                throw new InvalidOperationException("Out of handles!");
            }

            // _transferMem.Storage.Write(0, UpdateBuffer());

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(handle);

            _callerAppletCaptureBufferAcquired = true;

            context.ResponseData.Write(_callerAppletCaptureBufferAcquired);

            return ResultCode.Success;
        }
    }
}