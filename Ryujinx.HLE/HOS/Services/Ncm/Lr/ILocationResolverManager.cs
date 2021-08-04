using Ryujinx.Common;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.HOS.Services.Ncm.Lr.LocationResolverManager;

namespace Ryujinx.HLE.HOS.Services.Ncm.Lr
{
    [Service("lr")]
    class ILocationResolverManager : IpcService
    {
        [CommandHipc(0)]
        // OpenLocationResolver()
        public ResultCode OpenLocationResolver(ServiceCtx context)
        {
            StorageId storageId = context.RequestData.ReadStruct<StorageId>();

            MakeObject(context, new ILocationResolver(storageId));

            return ResultCode.Success;
        }
    }
}