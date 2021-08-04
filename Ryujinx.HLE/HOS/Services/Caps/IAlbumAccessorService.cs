using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Caps
{
    [Service("caps:a")]
    class IAlbumAccessorService : IpcService
    {
        [CommandHipc(18)]
        // Unk()
        public ResultCode Unk(ServiceCtx context)
        {
            // TODO: find out anything about this command :P

            Logger.Stub?.PrintStub(LogClass.ServiceCaps);

            return ResultCode.Success;
        }
    }
}