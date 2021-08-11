using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Kernel.Process;
using System.Collections.Concurrent;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    public class ApplicationContext : AppletContextBase
    {
        private ConcurrentDictionary<LaunchParameterKind, byte[]> _launchParameters;

        public ulong ApplicationId { get; private set; }

        public bool IsSystem { get; private set; }

        public ApplicationContext(long callerProcessId, ulong applicationId, bool isSystem) : base(callerProcessId, isSystem ? AppletId.SystemApplication : AppletId.Application)
        {
            ApplicationId = applicationId;
            IsSystem = isSystem;
        }

        public void PushLaunchParameter(LaunchParameterKind kind, byte[] data)
        {
            _launchParameters.AddOrUpdate(kind, (LaunchParameterKind kind) => data, (LaunchParameterKind KContextIdManager, byte[] oldData) => data);
        }

        public bool TryPopLaunchParameter(LaunchParameterKind kind, out byte[] data)
        {
            return _launchParameters.TryRemove(kind, out data);
        }

        public override ResultCode Start()
        {
            // Logger.Info?.Print(LogClass.ServiceAm, "Starting application...");

            var processId = Horizon.Instance.Device.Application.LoadApplication(this);
            if (processId < 0)
            {
                return ResultCode.AppletLaunchFailed;
            }
            else
            {
                return ResultCode.Success;
            }
        }
    }
}
