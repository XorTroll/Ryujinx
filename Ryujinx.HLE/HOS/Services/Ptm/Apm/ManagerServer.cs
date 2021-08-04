﻿namespace Ryujinx.HLE.HOS.Services.Ptm.Apm
{
    [Service("apm")]
    [Service("apm:am")] // 8.0.0+
    class ManagerServer : IManager
    {
        protected override ResultCode OpenSession(out SessionServer sessionServer)
        {
            sessionServer = new SessionServer();

            return ResultCode.Success;
        }

        protected override PerformanceMode GetPerformanceMode()
        {
            return Horizon.Instance.PerformanceState.PerformanceMode;
        }

        protected override bool IsCpuOverclockEnabled()
        {
            return Horizon.Instance.PerformanceState.CpuOverclockEnabled;
        }
    }
}