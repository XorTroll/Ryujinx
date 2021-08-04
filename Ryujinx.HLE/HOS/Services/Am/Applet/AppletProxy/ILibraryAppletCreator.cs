using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Kernel.Memory;
using Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy.LibraryAppletCreator;

namespace Ryujinx.HLE.HOS.Services.Am.Applet.AppletProxy
{
    class ILibraryAppletCreator : IpcService
    {
        [CommandHipc(0)]
        // CreateLibraryApplet(u32, u32) -> object<nn::am::service::ILibraryAppletAccessor>
        public ResultCode CreateLibraryApplet(ServiceCtx context)
        {
            var appletId = (AppletId)context.RequestData.ReadInt32();
            var libraryAppletMode = (LibraryAppletMode)context.RequestData.ReadInt32();

            Logger.Stub?.PrintStub(LogClass.ServiceAm, new { appletId, libraryAppletMode });

            MakeObject(context, new ILibraryAppletAccessor(appletId));

            return ResultCode.Success;
        }

        [CommandHipc(10)]
        // CreateStorage(u64) -> object<nn::am::service::IStorage>
        public ResultCode CreateStorage(ServiceCtx context)
        {
            long size = context.RequestData.ReadInt64();

            if (size <= 0)
            {
                return ResultCode.ObjectInvalid;
            }

            MakeObject(context, new IStorage(new byte[size]));

            // NOTE: Returns ResultCode.MemoryAllocationFailed if IStorage is null, it doesn't occur in our case.

            return ResultCode.Success;
        }

        [CommandHipc(11)]
        // CreateTransferMemoryStorage(b8, u64, handle<copy>) -> object<nn::am::service::IStorage>
        public ResultCode CreateTransferMemoryStorage(ServiceCtx context)
        {
            bool isReadOnly = (context.RequestData.ReadInt64() & 1) == 0;
            long size       = context.RequestData.ReadInt64();
            int  handle     = context.Request.HandleDesc.ToCopy[0];

            KTransferMemory transferMem = context.Process.HandleTable.GetObject<KTransferMemory>(handle);

            if (size <= 0)
            {
                return ResultCode.ObjectInvalid;
            }

            byte[] data = new byte[transferMem.Size];

            transferMem.Creator.CpuMemory.Read(transferMem.Address, data);

            Horizon.Instance.KernelContext.Syscall.CloseHandle(handle);

            MakeObject(context, new IStorage(data, isReadOnly));

            return ResultCode.Success;
        }

        [CommandHipc(12)] // 2.0.0+
        // CreateHandleStorage(u64, handle<copy>) -> object<nn::am::service::IStorage>
        public ResultCode CreateHandleStorage(ServiceCtx context)
        {
            long size   = context.RequestData.ReadInt64();
            int  handle = context.Request.HandleDesc.ToCopy[0];

            KTransferMemory transferMem = context.Process.HandleTable.GetObject<KTransferMemory>(handle);

            if (size <= 0)
            {
                return ResultCode.ObjectInvalid;
            }

            byte[] data = new byte[transferMem.Size];

            transferMem.Creator.CpuMemory.Read(transferMem.Address, data);

            Horizon.Instance.KernelContext.Syscall.CloseHandle(handle);

            MakeObject(context, new IStorage(data));

            return ResultCode.Success;
        }
    }
}