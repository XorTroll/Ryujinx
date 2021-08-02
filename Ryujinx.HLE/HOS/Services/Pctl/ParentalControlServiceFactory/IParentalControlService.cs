using LibHac.Ns;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services.Glue.Arp;
using System;

namespace Ryujinx.HLE.HOS.Services.Pctl.ParentalControlServiceFactory
{
    class IParentalControlService : IpcService
    {
        private long                     _pid;
        private int                      _permissionFlag;
        private ulong                    _titleId;
        private ParentalControlFlagValue _parentalControlFlag;
        private int[]                    _ratingAge;

        // TODO: Find where they are set.
        private bool _restrictionEnabled                  = false;
        private bool _featuresRestriction                 = false;
        private bool _stereoVisionRestrictionConfigurable = true;
        private bool _stereoVisionRestriction             = false;

        private KEvent _synchronizationEvent;
        private int    _synchronizationEventHandle;
        
        private KEvent _unlinkedEvent;
        private int _unlinkedEventHandle;

        private KEvent _playTimerEvent;
        private int _playTimerEventHandle;

        public IParentalControlService(ServiceCtx context, long pid, bool withInitialize, int permissionFlag)
        {
            _pid            = pid;
            _permissionFlag = permissionFlag;
            _synchronizationEvent = new KEvent(context.Device.System.KernelContext);
            _unlinkedEvent = new KEvent(context.Device.System.KernelContext);
            _playTimerEvent = new KEvent(context.Device.System.KernelContext);

            if (withInitialize)
            {
                Initialize(context);
            }
        }

        [CommandHipc(1)] // 4.0.0+
        // Initialize()
        public ResultCode Initialize(ServiceCtx context)
        {
            if ((_permissionFlag & 0x8001) == 0)
            {
                return ResultCode.PermissionDenied;
            }

            ResultCode resultCode = ResultCode.InvalidPid;

            if (_pid != 0)
            {
                if ((_permissionFlag & 0x40) == 0)
                {
                    ulong titleId = ApplicationLaunchProperty.GetByPid(context).TitleId;

                    if (titleId != 0)
                    {
                        _titleId = titleId;

                        // TODO: Call nn::arp::GetApplicationControlProperty here when implemented, if it return ResultCode.Success we assign fields.
                        _ratingAge           = Array.ConvertAll(context.Device.Application.ControlData.Value.RatingAge.ToArray(), Convert.ToInt32);
                        _parentalControlFlag = context.Device.Application.ControlData.Value.ParentalControl;
                    }
                }

                if (_titleId != 0)
                {
                    // TODO: Service store some private fields in another static object.

                    if ((_permissionFlag & 0x8040) == 0)
                    {
                        // TODO: Service store TitleId and FreeCommunicationEnabled in another static object.
                        //       When it's done it signal an event in this static object.
                        Logger.Stub?.PrintStub(LogClass.ServicePctl);
                    }
                }

                resultCode = ResultCode.Success;
            }

            return resultCode;
        }

        [CommandHipc(1001)]
        // CheckFreeCommunicationPermission()
        public ResultCode CheckFreeCommunicationPermission(ServiceCtx context)
        {
            if (_parentalControlFlag == ParentalControlFlagValue.FreeCommunication && _restrictionEnabled)
            {
                // TODO: It seems to checks if an entry exists in the FreeCommunicationApplicationList using the TitleId.
                //       Then it returns FreeCommunicationDisabled if the entry doesn't exist.

                return ResultCode.FreeCommunicationDisabled;
            }

            // NOTE: This sets an internal field to true. Usage have to be determined.

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1006)]
        // IsRestrictionTemporaryUnlocked() -> bool
        public ResultCode IsRestrictionTemporaryUnlocked(ServiceCtx context)
        {
            // TODO

            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1010)]
        // IsRestrictedSystemSettingsEntered() -> bool
        public ResultCode IsRestrictedSystemSettingsEntered(ServiceCtx context)
        {
            // TODO

            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1013)] // 4.0.0+
        // ConfirmStereoVisionPermission()
        public ResultCode ConfirmStereoVisionPermission(ServiceCtx context)
        {
            return IsStereoVisionPermittedImpl();
        }

        [CommandHipc(1018)]
        // IsFreeCommunicationAvailable()
        public ResultCode IsFreeCommunicationAvailable(ServiceCtx context)
        {
            if (_parentalControlFlag == ParentalControlFlagValue.FreeCommunication && _restrictionEnabled)
            {
                // TODO: It seems to checks if an entry exists in the FreeCommunicationApplicationList using the TitleId.
                //       Then it returns FreeCommunicationDisabled if the entry doesn't exist.

                return ResultCode.FreeCommunicationDisabled;
            }

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.FreeCommunicationDisabled;
        }

        [CommandHipc(1031)]
        // IsRestrictionEnabled() -> b8
        public ResultCode IsRestrictionEnabled(ServiceCtx context)
        {
            if ((_permissionFlag & 0x140) == 0)
            {
                return ResultCode.PermissionDenied;
            }

            context.ResponseData.Write(_restrictionEnabled);

            return ResultCode.Success;
        }

        [CommandHipc(1032)]
        // GetSafetyLevel() -> u32
        public ResultCode GetSafetyLevel(ServiceCtx context)
        {
            // TODO
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1035)]
        // GetCurrentSettings() -> nn::pctl::RestrictionSettings
        public ResultCode GetCurrentSettings(ServiceCtx context)
        {
            // TODO
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1039)]
        // GetFreeCommunicationApplicationListCount() -> u32
        public ResultCode GetFreeCommunicationApplicationListCount(ServiceCtx context)
        {
            // TODO
            context.ResponseData.Write((uint)1);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1061)] // 4.0.0+
        // ConfirmStereoVisionRestrictionConfigurable()
        public ResultCode ConfirmStereoVisionRestrictionConfigurable(ServiceCtx context)
        {
            if ((_permissionFlag & 2) == 0)
            {
                return ResultCode.PermissionDenied;
            }

            if (_stereoVisionRestrictionConfigurable)
            {
                return ResultCode.Success;
            }
            else
            {
                return ResultCode.StereoVisionRestrictionConfigurableDisabled;
            }
        }

        [CommandHipc(1062)] // 4.0.0+
        // GetStereoVisionRestriction() -> bool
        public ResultCode GetStereoVisionRestriction(ServiceCtx context)
        {
            if ((_permissionFlag & 0x200) == 0)
            {
                return ResultCode.PermissionDenied;
            }

            bool stereoVisionRestriction = false;

            if (_stereoVisionRestrictionConfigurable)
            {
                stereoVisionRestriction = _stereoVisionRestriction;
            }

            context.ResponseData.Write(stereoVisionRestriction);

            return ResultCode.Success;
        }

        [CommandHipc(1063)] // 4.0.0+
        // SetStereoVisionRestriction(bool)
        public ResultCode SetStereoVisionRestriction(ServiceCtx context)
        {
            if ((_permissionFlag & 0x200) == 0)
            {
                return ResultCode.PermissionDenied;
            }

            bool stereoVisionRestriction = context.RequestData.ReadBoolean();

            if (!_featuresRestriction)
            {
                if (_stereoVisionRestrictionConfigurable)
                {
                    _stereoVisionRestriction = stereoVisionRestriction;

                    // TODO: It signals an internal event of service. We have to determine where this event is used. 
                }
            }

            return ResultCode.Success;
        }

        [CommandHipc(1064)] // 5.0.0+
        // ResetConfirmedStereoVisionPermission()
        public ResultCode ResetConfirmedStereoVisionPermission(ServiceCtx context)
        {
            return ResultCode.Success;
        }

        [CommandHipc(1065)] // 5.0.0+
        // IsStereoVisionPermitted() -> bool
        public ResultCode IsStereoVisionPermitted(ServiceCtx context)
        {
            bool isStereoVisionPermitted = false;

            ResultCode resultCode = IsStereoVisionPermittedImpl();

            if (resultCode == ResultCode.Success)
            {
                isStereoVisionPermitted = true;
            }

            context.ResponseData.Write(isStereoVisionPermitted);

            return resultCode;
        }

        [CommandHipc(1403)]
        // IsPairingActive() -> bool
        public ResultCode IsPairingActive(ServiceCtx context)
        {
            // TODO
            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1432)]
        // GetSynchronizationEvent() -> handle<copy>
        public ResultCode GetSynchronizationEvent(ServiceCtx context)
        {
            if (_synchronizationEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_synchronizationEvent.ReadableEvent, out _synchronizationEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_synchronizationEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1451)]
        // StartPlayTimer()
        public ResultCode StartPlayTimer(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1452)]
        // StopPlayTimer()
        public ResultCode StopPlayTimer(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1455)]
        // IsRestrictedByPlayTimer() -> bool
        public ResultCode IsRestrictedByPlayTimer(ServiceCtx context)
        {
            // TODO

            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1456)]
        // GetPlayTimerSettings() -> nn::pctl::PlayTimerSettings
        public ResultCode GetPlayTimerSettings(ServiceCtx context)
        {
            var play_timer_settings = new byte[0x34];
            context.ResponseData.Write(play_timer_settings);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1457)]
        // GetPlayTimerEventToRequestSuspension() -> handle<copy>
        public ResultCode GetPlayTimerEventToRequestSuspension(ServiceCtx context)
        {
            if (_playTimerEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_playTimerEvent.ReadableEvent, out _playTimerEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_playTimerEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1458)]
        // IsPlayTimerAlarmDisabled() -> bool
        public ResultCode IsPlayTimerAlarmDisabled(ServiceCtx context)
        {
            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        [CommandHipc(1473)]
        // GetUnlinkedEvent() -> handle<copy>
        public ResultCode GetUnlinkedEvent(ServiceCtx context)
        {
            if (_unlinkedEventHandle == 0)
            {
                if (context.Process.HandleTable.GenerateHandle(_unlinkedEvent.ReadableEvent, out _unlinkedEventHandle) != KernelResult.Success)
                {
                    throw new InvalidOperationException("Out of handles!");
                }
            }

            context.Response.HandleDesc = IpcHandleDesc.MakeCopy(_unlinkedEventHandle);

            Logger.Stub?.PrintStub(LogClass.ServicePctl);

            return ResultCode.Success;
        }

        private ResultCode IsStereoVisionPermittedImpl()
        {
            /*
                // TODO: Application Exemptions are readed from file "appExemptions.dat" in the service savedata.
                //       Since we don't support the pctl savedata for now, this can be implemented later.

                if (appExemption)
                {
                    return ResultCode.Success;
                }
            */

            if (_stereoVisionRestrictionConfigurable && _stereoVisionRestriction)
            {
                return ResultCode.StereoVisionDenied;
            }
            else
            {
                return ResultCode.Success;
            }
        }
    }
}