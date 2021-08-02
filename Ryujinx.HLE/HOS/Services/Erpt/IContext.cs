﻿using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Erpt
{
    [Service("erpt:c")]
    class IContext : IpcService
    {
        public IContext() { }

        [CommandHipc(0)]
        // SubmitContext()
        public ResultCode SubmitContext(ServiceCtx context)
        {
            // TODO: error report context format parsing, etc.

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }
    }
}