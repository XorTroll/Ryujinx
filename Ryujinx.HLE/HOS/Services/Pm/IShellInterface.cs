namespace Ryujinx.HLE.HOS.Services.Pm
{
    [Service("pm:shell")]
    class IShellInterface : IpcService
    {
        [CommandHipc(6)]
        [CommandHipc(8)] // 7.0.0+
        // GetApplicationProcessIdForShell() -> u64
        public ResultCode GetApplicationProcessIdForShell(ServiceCtx context)
        {
            // FIXME: This is wrong but needed to make nx-hbloader work
            // TODO: Change this when we will have a way to process via a PM like interface.
            long pid = context.Process.Pid;

            context.ResponseData.Write(pid);

            return ResultCode.Success;
        }
    }
}
