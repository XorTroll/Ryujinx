using Ryujinx.HLE.HOS.Services.Fatal.Types;

namespace Ryujinx.HLE.HOS.Services.Fatal
{
    [Service("fatal:u")]
    class IService : IpcService
    {
        [CommandHipc(1)]
        // ThrowWithPolicy(pid, u32, u32)
        public ResultCode ThrowWithPolicy(ServiceCtx context)
        {
            var result = context.RequestData.ReadUInt32();
            var policy = (Policy)context.RequestData.ReadUInt32();

            HandleResultThrow(context, result, policy);

            return ResultCode.Success;
        }

        [CommandHipc(2)]
        // ThrowWithContext(pid, u32, u32, buffer<...>)
        public ResultCode ThrowWithContext(ServiceCtx context)
        {
            // TODO: specific implementation using CPU context
            return ThrowWithPolicy(context);
        }

        private void HandleResultThrow(ServiceCtx context, uint result, Policy policy)
        {
            if(result == 0)
            {
                // Program aborted
                result = 0x4a2;
            }

            context.Process.TerminateCurrentProcess();
            Horizon.Instance.Device.UiHandler.DisplayMessageDialog("Fatal", "Process " + context.Process.Pid.ToString("X16") + " fataled with error 0x" + result.ToString("X"));
        }
    }
}