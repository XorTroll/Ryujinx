using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ryujinx.HLE.HOS.Services;

namespace Ryujinx.HLE.HOS.Services.Ncm
{
    class IContentStorage : IpcService
    {
        public IContentStorage(ServiceCtx context) { }

        [CommandHipc(8)]
        // GetPath(ContentId)
        public ResultCode GetPath(ServiceCtx context)
        {
            // MakeObject(context, new ISystemAppletProxy(context.Request.HandleDesc.PId));

            return ResultCode.Success;
        }
    }
}
