using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Ptm.Apm
{
    class SessionServer : ISession
    {
        private readonly Horizon _system;

        public SessionServer(Horizon system)
        {
            _system = system;
        }

        protected override ResultCode SetPerformanceConfiguration(PerformanceMode performanceMode, PerformanceConfiguration performanceConfiguration)
        {
            if (performanceMode > PerformanceMode.Boost)
            {
                return ResultCode.InvalidParameters;
            }

            switch (performanceMode)
            {
                case PerformanceMode.Default:
                    _system.PerformanceState.DefaultPerformanceConfiguration = performanceConfiguration;
                    break;
                case PerformanceMode.Boost:
                    _system.PerformanceState.BoostPerformanceConfiguration = performanceConfiguration;
                    break;
                default:
                    Logger.Error?.Print(LogClass.ServiceApm, $"PerformanceMode isn't supported: {performanceMode}");
                    break;
            }

            return ResultCode.Success;
        }

        protected override ResultCode GetPerformanceConfiguration(PerformanceMode performanceMode, out PerformanceConfiguration performanceConfiguration)
        {
            if (performanceMode > PerformanceMode.Boost)
            {
                performanceConfiguration = 0;

                return ResultCode.InvalidParameters;
            }

            performanceConfiguration = _system.PerformanceState.GetCurrentPerformanceConfiguration(performanceMode);

            return ResultCode.Success;
        }

        protected override void SetCpuOverclockEnabled(bool enabled)
        {
            _system.PerformanceState.CpuOverclockEnabled = enabled;

            // NOTE: This call seems to overclock the system, since we emulate it, it's fine to do nothing instead.
        }
    }
}