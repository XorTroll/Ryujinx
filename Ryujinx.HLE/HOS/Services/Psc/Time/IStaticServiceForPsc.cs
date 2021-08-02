using Ryujinx.Common;
using Ryujinx.Cpu;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services.Time;
using Ryujinx.HLE.HOS.Services.Time.Clock;
using Ryujinx.HLE.HOS.Services.Time.StaticService;
using Ryujinx.HLE.HOS.Services.Time.TimeZone;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using TimeResultCode = Ryujinx.HLE.HOS.Services.Time.ResultCode;

namespace Ryujinx.HLE.HOS.Services.Psc.Time
{
    [Service("time:s", TimePermissions.System)]
    [Service("time:su", TimePermissions.SystemUpdate)]
    class IStaticServiceForPsc : IpcService
    {
        private TimeManager     _timeManager;
        private TimePermissions _permissions;

        private int _timeSharedMemoryNativeHandle = 0;

        public IStaticServiceForPsc(TimeManager manager, TimePermissions permissions)
        {
            _permissions = permissions;
            _timeManager = manager;
        }

        [CommandHipc(0)]
        // GetStandardUserSystemClock() -> object<nn::timesrv::detail::service::ISystemClock>
        public TimeResultCode GetStandardUserSystemClock(ServiceCtx context)
        {
            MakeObject(context, new ISystemClock(_timeManager.StandardUserSystemClock,
                (_permissions & TimePermissions.UserSystemClockWritableMask) != 0,
                (_permissions & TimePermissions.BypassUninitialized) != 0));

            return TimeResultCode.Success;
        }

        [CommandHipc(1)]
        // GetStandardNetworkSystemClock() -> object<nn::timesrv::detail::service::ISystemClock>
        public TimeResultCode GetStandardNetworkSystemClock(ServiceCtx context)
        {
            MakeObject(context, new ISystemClock(_timeManager.StandardNetworkSystemClock,
                (_permissions & TimePermissions.NetworkSystemClockWritableMask) != 0,
                (_permissions & TimePermissions.BypassUninitialized) != 0));

            return TimeResultCode.Success;
        }

        [CommandHipc(2)]
        // GetStandardSteadyClock() -> object<nn::timesrv::detail::service::ISteadyClock>
        public TimeResultCode GetStandardSteadyClock(ServiceCtx context)
        {
            MakeObject(context, new ISteadyClock(_timeManager.StandardSteadyClock,
                (_permissions & TimePermissions.SteadyClockWritableMask) != 0,
                (_permissions & TimePermissions.BypassUninitialized) != 0));

            return TimeResultCode.Success;
        }

        [CommandHipc(3)]
        // GetTimeZoneService() -> object<nn::timesrv::detail::service::ITimeZoneService>
        public TimeResultCode GetTimeZoneService(ServiceCtx context)
        {
            MakeObject(context, new ITimeZoneServiceForPsc(_timeManager.TimeZone.Manager,
                (_permissions & TimePermissions.TimeZoneWritableMask) != 0));

            return TimeResultCode.Success;
        }

        [CommandHipc(4)]
        // GetStandardLocalSystemClock() -> object<nn::timesrv::detail::service::ISystemClock>
        public TimeResultCode GetStandardLocalSystemClock(ServiceCtx context)
        {
            MakeObject(context, new ISystemClock(_timeManager.StandardLocalSystemClock,
                (_permissions & TimePermissions.LocalSystemClockWritableMask) != 0,
                (_permissions & TimePermissions.BypassUninitialized) != 0));

            return TimeResultCode.Success;
        }

        [CommandHipc(5)] // 4.0.0+
        // GetEphemeralNetworkSystemClock() -> object<nn::timesrv::detail::service::ISystemClock>
        public TimeResultCode GetEphemeralNetworkSystemClock(ServiceCtx context)
        {
            MakeObject(context, new ISystemClock(_timeManager.StandardNetworkSystemClock,
                (_permissions & TimePermissions.NetworkSystemClockWritableMask) != 0,
                (_permissions & TimePermissions.BypassUninitialized) != 0));

            return TimeResultCode.Success;
        }

        [CommandHipc(20)] // 6.0.0+
        // GetSharedMemoryNativeHandle() -> handle<copy>
        public TimeResultCode GetSharedMemoryNativeHandle(ServiceCtx context)
        {
            if (_timeSharedMemoryNativeHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_timeManager.SharedMemory.GetSharedMemory(), out _timeSharedMemoryNativeHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_timeSharedMemoryNativeHandle);

            return TimeResultCode.Success;
        }

        [CommandHipc(50)] // 4.0.0+
        // SetStandardSteadyClockInternalOffset(nn::TimeSpanType internal_offset)
        public TimeResultCode SetStandardSteadyClockInternalOffset(ServiceCtx context)
        {
            // This is only implemented in glue's StaticService.
            return TimeResultCode.NotImplemented;
        }

        [CommandHipc(51)] // 9.0.0+
        // GetStandardSteadyClockRtcValue() -> u64
        public TimeResultCode GetStandardSteadyClockRtcValue(ServiceCtx context)
        {
            // This is only implemented in glue's StaticService.
            return TimeResultCode.NotImplemented;
        }

        [CommandHipc(100)]
        // IsStandardUserSystemClockAutomaticCorrectionEnabled() -> bool
        public TimeResultCode IsStandardUserSystemClockAutomaticCorrectionEnabled(ServiceCtx context)
        {
            StandardUserSystemClockCore userClock = _timeManager.StandardUserSystemClock;

            if (!userClock.IsInitialized())
            {
                return TimeResultCode.UninitializedClock;
            }

            context.ResponseData.Write(userClock.IsAutomaticCorrectionEnabled());

            return TimeResultCode.Success;
        }

        [CommandHipc(101)]
        // SetStandardUserSystemClockAutomaticCorrectionEnabled(b8)
        public TimeResultCode SetStandardUserSystemClockAutomaticCorrectionEnabled(ServiceCtx context)
        {
            SteadyClockCore             steadyClock = _timeManager.StandardSteadyClock;
            StandardUserSystemClockCore userClock   = _timeManager.StandardUserSystemClock;

            if (!userClock.IsInitialized() || !steadyClock.IsInitialized())
            {
                return TimeResultCode.UninitializedClock;
            }

            if ((_permissions & TimePermissions.UserSystemClockWritableMask) == 0)
            {
                return TimeResultCode.PermissionDenied;
            }

            bool autoCorrectionEnabled = context.RequestData.ReadBoolean();

            var result = userClock.SetAutomaticCorrectionEnabled(context.Thread, autoCorrectionEnabled);

            if (result == TimeResultCode.Success)
            {
                _timeManager.SharedMemory.SetAutomaticCorrectionEnabled(autoCorrectionEnabled);

                SteadyClockTimePoint currentTimePoint = userClock.GetSteadyClockCore().GetCurrentTimePoint(context.Thread);

                userClock.SetAutomaticCorrectionUpdatedTime(currentTimePoint);
                userClock.SignalAutomaticCorrectionEvent();
            }

            return result;
        }

        [CommandHipc(102)] // 5.0.0+
        // GetStandardUserSystemClockInitialYear() -> u32
        public TimeResultCode GetStandardUserSystemClockInitialYear(ServiceCtx context)
        {
            // This is only implemented in glue's StaticService.
            return TimeResultCode.NotImplemented;
        }

        [CommandHipc(200)] // 3.0.0+
        // IsStandardNetworkSystemClockAccuracySufficient() -> bool
        public TimeResultCode IsStandardNetworkSystemClockAccuracySufficient(ServiceCtx context)
        {
            context.ResponseData.Write(_timeManager.StandardNetworkSystemClock.IsStandardNetworkSystemClockAccuracySufficient(context.Thread));

            return TimeResultCode.Success;
        }

        [CommandHipc(201)] // 6.0.0+
        // GetStandardUserSystemClockAutomaticCorrectionUpdatedTime() -> nn::time::SteadyClockTimePoint
        public TimeResultCode GetStandardUserSystemClockAutomaticCorrectionUpdatedTime(ServiceCtx context)
        {
            StandardUserSystemClockCore userClock = _timeManager.StandardUserSystemClock;

            if (!userClock.IsInitialized())
            {
                return TimeResultCode.UninitializedClock;
            }

            context.ResponseData.WriteStruct(userClock.GetAutomaticCorrectionUpdatedTime());

            return TimeResultCode.Success;
        }

        [CommandHipc(300)] // 4.0.0+
        // CalculateMonotonicSystemClockBaseTimePoint(nn::time::SystemClockContext) -> s64
        public TimeResultCode CalculateMonotonicSystemClockBaseTimePoint(ServiceCtx context)
        {
            SteadyClockCore steadyClock = _timeManager.StandardSteadyClock;

            if (!steadyClock.IsInitialized())
            {
                return TimeResultCode.UninitializedClock;
            }

            SystemClockContext   otherContext     = context.RequestData.ReadStruct<SystemClockContext>();
            SteadyClockTimePoint currentTimePoint = steadyClock.GetCurrentTimePoint(context.Thread);

            var result = TimeResultCode.TimeMismatch;

            if (currentTimePoint.ClockSourceId == otherContext.SteadyTimePoint.ClockSourceId)
            {
                TimeSpanType ticksTimeSpan = TimeSpanType.FromTicks(context.Thread.Context.CntpctEl0, context.Thread.Context.CntfrqEl0);
                long         baseTimePoint = otherContext.Offset + currentTimePoint.TimePoint - ticksTimeSpan.ToSeconds();

                context.ResponseData.Write(baseTimePoint);

                result = TimeResultCode.Success;
            }

            return result;
        }

        [CommandHipc(400)] // 4.0.0+
        // GetClockSnapshot(u8) -> buffer<nn::time::sf::ClockSnapshot, 0x1a>
        public TimeResultCode GetClockSnapshot(ServiceCtx context)
        {
            byte type = context.RequestData.ReadByte();

            context.Response.PtrBuff[0] = context.Response.PtrBuff[0].WithSize((uint)Marshal.SizeOf<ClockSnapshot>());

            var result = _timeManager.StandardUserSystemClock.GetClockContext(context.Thread, out SystemClockContext userContext);

            if (result == TimeResultCode.Success)
            {
                result = _timeManager.StandardNetworkSystemClock.GetClockContext(context.Thread, out SystemClockContext networkContext);

                if (result == TimeResultCode.Success)
                {
                    result = GetClockSnapshotFromSystemClockContextInternal(context.Thread, userContext, networkContext, type, out ClockSnapshot clockSnapshot);

                    if (result == TimeResultCode.Success)
                    {
                        WriteClockSnapshotFromBuffer(context, context.Request.RecvListBuff[0], clockSnapshot);
                    }
                }
            }

            return result;
        }

        [CommandHipc(401)] // 4.0.0+
        // GetClockSnapshotFromSystemClockContext(u8, nn::time::SystemClockContext, nn::time::SystemClockContext) -> buffer<nn::time::sf::ClockSnapshot, 0x1a>
        public TimeResultCode GetClockSnapshotFromSystemClockContext(ServiceCtx context)
        {
            byte type = context.RequestData.ReadByte();

            context.Response.PtrBuff[0] = context.Response.PtrBuff[0].WithSize((uint)Marshal.SizeOf<ClockSnapshot>());

            context.RequestData.BaseStream.Position += 7;

            SystemClockContext userContext    = context.RequestData.ReadStruct<SystemClockContext>();
            SystemClockContext networkContext = context.RequestData.ReadStruct<SystemClockContext>();

            var result = GetClockSnapshotFromSystemClockContextInternal(context.Thread, userContext, networkContext, type, out ClockSnapshot clockSnapshot);

            if (result == TimeResultCode.Success)
            {
                WriteClockSnapshotFromBuffer(context, context.Request.RecvListBuff[0], clockSnapshot);
            }

            return result;
        }

        [CommandHipc(500)] // 4.0.0+
        // CalculateStandardUserSystemClockDifferenceByUser(buffer<nn::time::sf::ClockSnapshot, 0x19>, buffer<nn::time::sf::ClockSnapshot, 0x19>) -> nn::TimeSpanType
        public TimeResultCode CalculateStandardUserSystemClockDifferenceByUser(ServiceCtx context)
        {
            ClockSnapshot clockSnapshotA = ReadClockSnapshotFromBuffer(context, context.Request.PtrBuff[0]);
            ClockSnapshot clockSnapshotB = ReadClockSnapshotFromBuffer(context, context.Request.PtrBuff[1]);
            TimeSpanType  difference     = TimeSpanType.FromSeconds(clockSnapshotB.UserContext.Offset - clockSnapshotA.UserContext.Offset);

            if (clockSnapshotB.UserContext.SteadyTimePoint.ClockSourceId != clockSnapshotA.UserContext.SteadyTimePoint.ClockSourceId || (clockSnapshotB.IsAutomaticCorrectionEnabled && clockSnapshotA.IsAutomaticCorrectionEnabled))
            {
                difference = new TimeSpanType(0);
            }

            context.ResponseData.Write(difference.NanoSeconds);

            return TimeResultCode.Success;
        }

        [CommandHipc(501)] // 4.0.0+
        // CalculateSpanBetween(buffer<nn::time::sf::ClockSnapshot, 0x19>, buffer<nn::time::sf::ClockSnapshot, 0x19>) -> nn::TimeSpanType
        public TimeResultCode CalculateSpanBetween(ServiceCtx context)
        {
            ClockSnapshot clockSnapshotA = ReadClockSnapshotFromBuffer(context, context.Request.PtrBuff[0]);
            ClockSnapshot clockSnapshotB = ReadClockSnapshotFromBuffer(context, context.Request.PtrBuff[1]);

            TimeSpanType result;

            var resultCode = clockSnapshotA.SteadyClockTimePoint.GetSpanBetween(clockSnapshotB.SteadyClockTimePoint, out long timeSpan);

            if (resultCode != TimeResultCode.Success)
            {
                resultCode = TimeResultCode.TimeNotFound;

                if (clockSnapshotA.NetworkTime != 0 && clockSnapshotB.NetworkTime != 0)
                {
                    result     = TimeSpanType.FromSeconds(clockSnapshotB.NetworkTime - clockSnapshotA.NetworkTime);
                    resultCode = TimeResultCode.Success;
                }
                else
                {
                    return resultCode;
                }
            }
            else
            {
                result = TimeSpanType.FromSeconds(timeSpan);
            }

            context.ResponseData.Write(result.NanoSeconds);

            return resultCode;
        }

        private TimeResultCode GetClockSnapshotFromSystemClockContextInternal(KThread thread, SystemClockContext userContext, SystemClockContext networkContext, byte type, out ClockSnapshot clockSnapshot)
        {
            clockSnapshot = new ClockSnapshot();

            SteadyClockCore      steadyClockCore  = _timeManager.StandardSteadyClock;
            SteadyClockTimePoint currentTimePoint = steadyClockCore.GetCurrentTimePoint(thread);

            clockSnapshot.IsAutomaticCorrectionEnabled = _timeManager.StandardUserSystemClock.IsAutomaticCorrectionEnabled();
            clockSnapshot.UserContext                  = userContext;
            clockSnapshot.NetworkContext               = networkContext;
            clockSnapshot.SteadyClockTimePoint         = currentTimePoint;

            var result = _timeManager.TimeZone.Manager.GetDeviceLocationName(out string deviceLocationName);

            if (result != TimeResultCode.Success)
            {
                return result;
            }

            char[] tzName       = deviceLocationName.ToCharArray();
            char[] locationName = new char[0x24];

            Array.Copy(tzName, locationName, tzName.Length);

            clockSnapshot.LocationName = locationName;

            result = ClockSnapshot.GetCurrentTime(out clockSnapshot.UserTime, currentTimePoint, clockSnapshot.UserContext);

            if (result == TimeResultCode.Success)
            {
                result = _timeManager.TimeZone.Manager.ToCalendarTimeWithMyRules(clockSnapshot.UserTime, out CalendarInfo userCalendarInfo);

                if (result == TimeResultCode.Success)
                {
                    clockSnapshot.UserCalendarTime           = userCalendarInfo.Time;
                    clockSnapshot.UserCalendarAdditionalTime = userCalendarInfo.AdditionalInfo;

                    if (ClockSnapshot.GetCurrentTime(out clockSnapshot.NetworkTime, currentTimePoint, clockSnapshot.NetworkContext) != TimeResultCode.Success)
                    {
                        clockSnapshot.NetworkTime = 0;
                    }

                    result = _timeManager.TimeZone.Manager.ToCalendarTimeWithMyRules(clockSnapshot.NetworkTime, out CalendarInfo networkCalendarInfo);

                    if (result == TimeResultCode.Success)
                    {
                        clockSnapshot.NetworkCalendarTime           = networkCalendarInfo.Time;
                        clockSnapshot.NetworkCalendarAdditionalTime = networkCalendarInfo.AdditionalInfo;
                        clockSnapshot.Type                          = type;

                        // Probably a version field?
                        clockSnapshot.Unknown = 0;
                    }
                }
            }

            return result;
        }

        private ClockSnapshot ReadClockSnapshotFromBuffer(ServiceCtx context, IpcPtrBuffDesc ipcDesc)
        {
            Debug.Assert(ipcDesc.Size == (ulong)Marshal.SizeOf<ClockSnapshot>());

            byte[] temp = new byte[ipcDesc.Size];

            context.Memory.Read(ipcDesc.Position, temp);

            using (BinaryReader bufferReader = new BinaryReader(new MemoryStream(temp)))
            {
                return bufferReader.ReadStruct<ClockSnapshot>();
            }
        }

        private void WriteClockSnapshotFromBuffer(ServiceCtx context, IpcRecvListBuffDesc ipcDesc, ClockSnapshot clockSnapshot)
        {
            MemoryHelper.Write(context.Memory, ipcDesc.Position, clockSnapshot);
        }
    }
}