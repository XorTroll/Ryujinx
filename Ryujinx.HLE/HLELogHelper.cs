using System;
using System.Threading;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.Utilities;

namespace Ryujinx.HLE
{
    public class HLELogHelper : ILogHelper
    {
        public string GetCurrentProcessName()
        {
            var currentProcess = KernelStatic.GetCurrentProcess();
            return currentProcess?.Name;
        }

        public string GetCurrentThreadName()
        {
            var currentThread = KernelStatic.GetCurrentThread();
            if(currentThread != null)
            {
                return currentThread.GetName();
            }
            return Thread.CurrentThread.Name;
        }
    }
}
