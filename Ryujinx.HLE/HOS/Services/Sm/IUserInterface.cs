using Ryujinx.Common.Logging;
using Ryujinx.HLE.Exceptions;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Ipc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ryujinx.HLE.HOS.Services.Sm
{
    class IUserInterface : IpcService
    {
        private bool _isInitialized;

        public IUserInterface() { }

        public SmServer Sm => Server as SmServer;

        [CommandHipc(0)]
        [CommandTipc(0)] // 12.0.0+
        // RegisterClient(pid)
        public ResultCode RegisterClient(ServiceCtx context)
        {
            _isInitialized = true;

            return ResultCode.Success;
        }

        [CommandHipc(1)]
        [CommandTipc(1)] // 12.0.0+
        // GetService(ServiceName name) -> handle<move, session>
        public ResultCode GetService(ServiceCtx context)
        {
            if (context.IsTipcProtocol)
            {
                context.Response.HandleDesc = IpcHandleDesc.MakeMove(0);
            }

            if (!_isInitialized)
            {
                return ResultCode.NotInitialized;
            }

            var name = ReadName(context);

            var rc = Sm.DoGetService(name, out var serviceHandle);
            context.Response.HandleDesc = IpcHandleDesc.MakeMove(serviceHandle);
            return rc;
        }

        [CommandHipc(2)]
        // RegisterService(ServiceName name, u8 isLight, u32 maxHandles) -> handle<move, port>
        [CommandTipc(2)] // 12.0.0+
        // RegisterService(ServiceName name, u32 maxHandles, u8 isLight) -> handle<move, port>
        public ResultCode RegisterService(ServiceCtx context)
        {
            throw new NotImplementedException("Registering actual services is currently not supported");
        }

        [CommandHipc(3)]
        [CommandTipc(3)] // 12.0.0+
        // UnregisterService(ServiceName name)
        public ResultCode UnregisterService(ServiceCtx context)
        {
            throw new NotImplementedException("Registering actual services is currently not supported");
        }

        private static string ReadName(ServiceCtx context)
        {
            string name = string.Empty;

            for (int index = 0; index < 8 &&
                context.RequestData.BaseStream.Position <
                context.RequestData.BaseStream.Length; index++)
            {
                byte chr = context.RequestData.ReadByte();

                if (chr >= 0x20 && chr < 0x7f)
                {
                    name += (char)chr;
                }
            }

            return name;
        }

        public override void DestroyAtExit()
        {
            // _commonServer.Dispose();

            base.DestroyAtExit();
        }
    }
}