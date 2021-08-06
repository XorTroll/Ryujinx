namespace Ryujinx.HLE.HOS.Kernel.Threading
{
    public enum SignalType
    {
        Signal                    = 0,
        SignalAndIncrementIfEqual = 1,
        SignalAndModifyIfEqual    = 2
    }
}
