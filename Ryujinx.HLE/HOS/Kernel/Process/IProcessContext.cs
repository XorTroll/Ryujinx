using ARMeilleure.State;
using Ryujinx.Memory;
using System;

namespace Ryujinx.HLE.HOS.Kernel.Process
{
    public interface IProcessContext : IDisposable
    {
        IVirtualMemoryManager AddressSpace { get; }

        void Execute(ExecutionContext context, ulong codeAddress);
    }
}
