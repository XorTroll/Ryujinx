namespace Ryujinx.HLE.HOS.Services.Ptm.Apm
{
    [Service("apm:sys")]
    class SystemManagerServer : ISystemManager
    {
        protected override void RequestPerformanceMode(PerformanceMode performanceMode)
        {
            Horizon.Instance.PerformanceState.PerformanceMode = performanceMode;
        }

        internal override void SetCpuBoostMode(CpuBoostMode cpuBoostMode)
        {
            Horizon.Instance.PerformanceState.CpuBoostMode = cpuBoostMode;
        }

        protected override PerformanceConfiguration GetCurrentPerformanceConfiguration()
        {
            return Horizon.Instance.PerformanceState.GetCurrentPerformanceConfiguration(Horizon.Instance.PerformanceState.PerformanceMode);
        }
    }
}