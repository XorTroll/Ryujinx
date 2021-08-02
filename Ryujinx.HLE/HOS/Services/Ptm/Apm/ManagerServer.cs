namespace Ryujinx.HLE.HOS.Services.Ptm.Apm
{
    [Service("apm")]
    [Service("apm:am")] // 8.0.0+
    class ManagerServer : IManager
    {
        private readonly Horizon _system;

        public ManagerServer(Horizon system)
        {
            _system = system;
        }

        protected override ResultCode OpenSession(out SessionServer sessionServer)
        {
            sessionServer = new SessionServer(_system);

            return ResultCode.Success;
        }

        protected override PerformanceMode GetPerformanceMode()
        {
            return _system.PerformanceState.PerformanceMode;
        }

        protected override bool IsCpuOverclockEnabled()
        {
            return _system.PerformanceState.CpuOverclockEnabled;
        }
    }
}