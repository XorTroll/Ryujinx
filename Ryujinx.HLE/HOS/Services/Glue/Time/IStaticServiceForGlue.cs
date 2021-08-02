using Ryujinx.Common;
using Ryujinx.HLE.HOS.Services.Pcv.Bpc;
using Ryujinx.HLE.HOS.Services.Settings;
using Ryujinx.HLE.HOS.Services.Time;
using Ryujinx.HLE.HOS.Services.Time.Clock;
using Ryujinx.HLE.HOS.Services.Time.StaticService;
using Ryujinx.HLE.HOS.Services.Psc.Time;
using System;

using TimeResultCode = Ryujinx.HLE.HOS.Services.Time.ResultCode;

namespace Ryujinx.HLE.HOS.Services.Glue.Time
{
    [Service("time:a", TimePermissions.Admin)]
    [Service("time:r", TimePermissions.Repair)]
    [Service("time:u", TimePermissions.User)]
    class IStaticServiceForGlue : IpcService
    {
        private IStaticServiceForPsc _inner;
        private TimePermissions      _permissions;

        public IStaticServiceForGlue(TimePermissions permissions)
        {
            _permissions = permissions;
            _inner       = new IStaticServiceForPsc(TimeManager.Instance, permissions);
            _inner.TrySetServer(Server);
            _inner.SetParent(this);
        }

        [CommandHipc(0)]
        // GetStandardUserSystemClock() -> object<nn::timesrv::detail::service::ISystemClock>
        public TimeResultCode GetStandardUserSystemClock(ServiceCtx context)
        {
            return _inner.GetStandardUserSystemClock(context);
        }

        [CommandHipc(1)]
        // GetStandardNetworkSystemClock() -> object<nn::timesrv::detail::service::ISystemClock>
        public TimeResultCode GetStandardNetworkSystemClock(ServiceCtx context)
        {
            return _inner.GetStandardNetworkSystemClock(context);
        }

        [CommandHipc(2)]
        // GetStandardSteadyClock() -> object<nn::timesrv::detail::service::ISteadyClock>
        public TimeResultCode GetStandardSteadyClock(ServiceCtx context)
        {
            return _inner.GetStandardSteadyClock(context);
        }

        [CommandHipc(3)]
        // GetTimeZoneService() -> object<nn::timesrv::detail::service::ITimeZoneService>
        public TimeResultCode GetTimeZoneService(ServiceCtx context)
        {
            MakeObject(context, new ITimeZoneServiceForGlue(TimeManager.Instance.TimeZone, (_permissions & TimePermissions.TimeZoneWritableMask) != 0));

            return TimeResultCode.Success;
        }

        [CommandHipc(4)]
        // GetStandardLocalSystemClock() -> object<nn::timesrv::detail::service::ISystemClock>
        public TimeResultCode GetStandardLocalSystemClock(ServiceCtx context)
        {
            return _inner.GetStandardLocalSystemClock(context);
        }

        [CommandHipc(5)] // 4.0.0+
        // GetEphemeralNetworkSystemClock() -> object<nn::timesrv::detail::service::ISystemClock>
        public TimeResultCode GetEphemeralNetworkSystemClock(ServiceCtx context)
        {
            return _inner.GetEphemeralNetworkSystemClock(context);
        }

        [CommandHipc(20)] // 6.0.0+
        // GetSharedMemoryNativeHandle() -> handle<copy>
        public TimeResultCode GetSharedMemoryNativeHandle(ServiceCtx context)
        {
            return _inner.GetSharedMemoryNativeHandle(context);
        }

        [CommandHipc(50)] // 4.0.0+
        // SetStandardSteadyClockInternalOffset(nn::TimeSpanType internal_offset)
        public TimeResultCode SetStandardSteadyClockInternalOffset(ServiceCtx context)
        {
            if ((_permissions & TimePermissions.SteadyClockWritableMask) == 0)
            {
                return TimeResultCode.PermissionDenied;
            }

            TimeSpanType internalOffset = context.RequestData.ReadStruct<TimeSpanType>();

            // TODO: set:sys SetExternalSteadyClockInternalOffset(internalOffset.ToSeconds())

            return TimeResultCode.Success;
        }

        [CommandHipc(51)] // 9.0.0+
        // GetStandardSteadyClockRtcValue() -> u64
        public TimeResultCode GetStandardSteadyClockRtcValue(ServiceCtx context)
        {
            var result = (TimeResultCode)IRtcManager.GetExternalRtcValue(out ulong rtcValue);

            if (result == TimeResultCode.Success)
            {
                context.ResponseData.Write(rtcValue);
            }

            return result;
        }

        [CommandHipc(100)]
        // IsStandardUserSystemClockAutomaticCorrectionEnabled() -> bool
        public TimeResultCode IsStandardUserSystemClockAutomaticCorrectionEnabled(ServiceCtx context)
        {
            return _inner.IsStandardUserSystemClockAutomaticCorrectionEnabled(context);
        }

        [CommandHipc(101)]
        // SetStandardUserSystemClockAutomaticCorrectionEnabled(b8)
        public TimeResultCode SetStandardUserSystemClockAutomaticCorrectionEnabled(ServiceCtx context)
        {
            return _inner.SetStandardUserSystemClockAutomaticCorrectionEnabled(context);
        }

        [CommandHipc(102)] // 5.0.0+
        // GetStandardUserSystemClockInitialYear() -> u32
        public TimeResultCode GetStandardUserSystemClockInitialYear(ServiceCtx context)
        {
            if (!NxSettings.Settings.TryGetValue("time!standard_user_clock_initial_year", out object standardUserSystemClockInitialYear))
            {
                throw new InvalidOperationException("standard_user_clock_initial_year isn't defined in system settings!");
            }

            context.ResponseData.Write((int)standardUserSystemClockInitialYear);

            return TimeResultCode.Success;
        }

        [CommandHipc(200)] // 3.0.0+
        // IsStandardNetworkSystemClockAccuracySufficient() -> bool
        public TimeResultCode IsStandardNetworkSystemClockAccuracySufficient(ServiceCtx context)
        {
            return _inner.IsStandardNetworkSystemClockAccuracySufficient(context);
        }

        [CommandHipc(201)] // 6.0.0+
        // GetStandardUserSystemClockAutomaticCorrectionUpdatedTime() -> nn::time::SteadyClockTimePoint
        public TimeResultCode GetStandardUserSystemClockAutomaticCorrectionUpdatedTime(ServiceCtx context)
        {
            return _inner.GetStandardUserSystemClockAutomaticCorrectionUpdatedTime(context);
        }

        [CommandHipc(300)] // 4.0.0+
        // CalculateMonotonicSystemClockBaseTimePoint(nn::time::SystemClockContext) -> s64
        public TimeResultCode CalculateMonotonicSystemClockBaseTimePoint(ServiceCtx context)
        {
            return _inner.CalculateMonotonicSystemClockBaseTimePoint(context);
        }

        [CommandHipc(400)] // 4.0.0+
        // GetClockSnapshot(u8) -> buffer<nn::time::sf::ClockSnapshot, 0x1a>
        public TimeResultCode GetClockSnapshot(ServiceCtx context)
        {
            return _inner.GetClockSnapshot(context);
        }

        [CommandHipc(401)] // 4.0.0+
        // GetClockSnapshotFromSystemClockContext(u8, nn::time::SystemClockContext, nn::time::SystemClockContext) -> buffer<nn::time::sf::ClockSnapshot, 0x1a>
        public TimeResultCode GetClockSnapshotFromSystemClockContext(ServiceCtx context)
        {
            return _inner.GetClockSnapshotFromSystemClockContext(context);
        }

        [CommandHipc(500)] // 4.0.0+
        // CalculateStandardUserSystemClockDifferenceByUser(buffer<nn::time::sf::ClockSnapshot, 0x19>, buffer<nn::time::sf::ClockSnapshot, 0x19>) -> nn::TimeSpanType
        public TimeResultCode CalculateStandardUserSystemClockDifferenceByUser(ServiceCtx context)
        {
            return _inner.CalculateStandardUserSystemClockDifferenceByUser(context);
        }

        [CommandHipc(501)] // 4.0.0+
        // CalculateSpanBetween(buffer<nn::time::sf::ClockSnapshot, 0x19>, buffer<nn::time::sf::ClockSnapshot, 0x19>) -> nn::TimeSpanType
        public TimeResultCode CalculateSpanBetween(ServiceCtx context)
        {
            return _inner.CalculateSpanBetween(context);
        }
    }
}
