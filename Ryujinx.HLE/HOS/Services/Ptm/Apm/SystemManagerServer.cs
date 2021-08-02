namespace Ryujinx.HLE.HOS.Services.Ptm.Apm
{
    [Service("apm:sys")]
    class SystemManagerServer : ISystemManager
    {
        private readonly Horizon _system;

        public SystemManagerServer(Horizon system)
        {
            _system = system;
        }

        protected override void RequestPerformanceMode(PerformanceMode performanceMode)
        {
            _system.PerformanceState.PerformanceMode = performanceMode;
        }

        internal override void SetCpuBoostMode(CpuBoostMode cpuBoostMode)
        {
            _system.PerformanceState.CpuBoostMode = cpuBoostMode;
        }

        protected override PerformanceConfiguration GetCurrentPerformanceConfiguration()
        {
            return _system.PerformanceState.GetCurrentPerformanceConfiguration(_system.PerformanceState.PerformanceMode);
        }
    }
}