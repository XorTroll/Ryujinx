using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Ptm.Apm
{
    class SessionServer : ISession
    {
        protected override ResultCode SetPerformanceConfiguration(PerformanceMode performanceMode, PerformanceConfiguration performanceConfiguration)
        {
            if (performanceMode > PerformanceMode.Boost)
            {
                return ResultCode.InvalidParameters;
            }

            switch (performanceMode)
            {
                case PerformanceMode.Default:
                    Horizon.Instance.PerformanceState.DefaultPerformanceConfiguration = performanceConfiguration;
                    break;
                case PerformanceMode.Boost:
                    Horizon.Instance.PerformanceState.BoostPerformanceConfiguration = performanceConfiguration;
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

            performanceConfiguration = Horizon.Instance.PerformanceState.GetCurrentPerformanceConfiguration(performanceMode);

            return ResultCode.Success;
        }

        protected override void SetCpuOverclockEnabled(bool enabled)
        {
            Horizon.Instance.PerformanceState.CpuOverclockEnabled = enabled;

            // NOTE: This call seems to overclock the system, since we emulate it, it's fine to do nothing instead.
        }
    }
}