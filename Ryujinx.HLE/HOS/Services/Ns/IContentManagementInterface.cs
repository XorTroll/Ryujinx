using Ryujinx.Common.Logging;
using Ryujinx.HLE.FileSystem;
using LibHac.Common;

namespace Ryujinx.HLE.HOS.Services.Ns
{
    class IContentManagementInterface : IpcService
    {
        [CommandHipc(43)]
        // CheckSdCardMountStatus()
        public ResultCode CheckSdCardMountStatus(ServiceCtx context)
        {
            // Note: Since in this emulator the SD card is always mounted, just do nothing (used by qlaunch to error if the SD card isn't accessible/mounted)
            // TODO: make use of this command to check if the SD card directory is not present for some reason?
            return ResultCode.Success;
        }

        [CommandHipc(47)]
        // GetTotalSpaceSize(nn::ncm::StorageId) -> long
        public ResultCode GetTotalSpaceSize(ServiceCtx context)
        {
            var storageId = (StorageId)context.RequestData.ReadByte();
            long totalSpace;

            switch (storageId)
            {
                case StorageId.SdCard:
                default: // TODO: implement others instead of returning SD values...
                    var fsProxy = Horizon.Instance.Device.FileSystem.FsServer.CreateFileSystemProxyService();

                    // TODO: check result codes here
                    fsProxy.OpenSdCardFileSystem(out var sdFs);
                    sdFs.GetTotalSpaceSize(out totalSpace, "/".ToU8Span());
                    break;
            }

            Logger.Stub?.PrintStub(LogClass.ServiceNs, new { storageId, totalSpace });
            context.ResponseData.Write(totalSpace);

            return ResultCode.Success;
        }

        [CommandHipc(48)]
        // GetFreeSpaceSize(nn::ncm::StorageId) -> long
        public ResultCode GetFreeSpaceSize(ServiceCtx context)
        {
            var storageId = (StorageId)context.RequestData.ReadByte();
            long freeSpace;

            switch(storageId)
            {
                case StorageId.SdCard:
                default: // TODO: implement others instead of returning SD values...
                    var fsProxy = Horizon.Instance.Device.FileSystem.FsServer.CreateFileSystemProxyService();

                    // TODO: check result codes here
                    fsProxy.OpenSdCardFileSystem(out var sdFs);
                    sdFs.GetFreeSpaceSize(out freeSpace, "/".ToU8Span());
                    break;
            }

            Logger.Stub?.PrintStub(LogClass.ServiceNs, new { storageId, freeSpace });
            context.ResponseData.Write(freeSpace);

            return ResultCode.Success;
        }
    }
}
