using Ryujinx.Memory;

namespace Ryujinx.HLE.HOS.Kernel.Process
{
    public interface IProcessContextFactory
    {
        IProcessContext Create(KernelContext context, long pid, ulong addressSpaceSize, InvalidAccessHandler invalidAccessHandler, bool for64Bit);
    }
}
