using Ryujinx.HLE.HOS;
using System;

namespace Ryujinx.HLE.Exceptions
{
    public class GuestBrokeExecutionException : Exception
    {
        public enum ExecutionBreakKind
        {
            FatalThrow,
            SvcBreak
        }

        private static string FormatMessage(ExecutionBreakKind kind, long processId, uint? result)
        {
            var processName = "<unk>";

            if (Horizon.Instance.KernelContext.Processes.TryGetValue(processId, out var process))
            {
                processName = process.Name; 
            }

            var msg = $"The process '{processName}' broke execution ";
            msg += "{ ";
            
            msg += $"Kind = {kind}";
            
            if (result != null)
            {
                msg += ", ";
                msg += $"Result = 0x{result ?? 0:X}";
            }
            
            msg += " }";
            return msg;
        }

        private void NotifyExecutionBroken()
        {
            // Show a message to the user before throwing the exception
            Horizon.Instance.Device.UiHandler.DisplayMessageDialog("Critical execution error", Message);
        }

        public GuestBrokeExecutionException(ExecutionBreakKind kind, long processId, uint? result) : base(FormatMessage(kind, processId, result))
        {
            NotifyExecutionBroken();
        }
    }
}