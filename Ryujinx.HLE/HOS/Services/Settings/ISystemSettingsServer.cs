using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;
using Ryujinx.Common;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.Utilities;
using Ryujinx.Cpu;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [Service("set:sys")]
    class ISystemSettingsServer : IpcService
    {
        private FirmwareVersion _firmwareVersion;
        private bool _firmwareVersionLoaded;

        private bool EnsureFirmwareVersionLoaded()
        {
            if(!_firmwareVersionLoaded)
            {
                _firmwareVersionLoaded = Horizon.Instance.ContentManager.TryGetCurrentFirmwareVersion(out _firmwareVersion);
            }

            return _firmwareVersionLoaded;
        }

        public ISystemSettingsServer()
        {
            _firmwareVersionLoaded = false;
        }

        [CommandHipc(0)]
        // SetLanguageCode(nn::settings::LanguageCode)
        public ResultCode SetLanguageCode(ServiceCtx context)
        {
            // TODO: set

            var langCode = context.RequestData.ReadUInt64();

            Logger.Stub?.PrintStub(LogClass.ServiceSet, new { langCode });

            return ResultCode.Success;
        }

        [CommandHipc(3)]
        // GetFirmwareVersion() -> buffer<nn::settings::system::FirmwareVersion, 0x1a, 0x100>
        public ResultCode GetFirmwareVersion(ServiceCtx context)
        {
            return GetFirmwareVersionImpl(context);
        }

        [CommandHipc(4)]
        // GetFirmwareVersion2() -> buffer<nn::settings::system::FirmwareVersion, 0x1a, 0x100>
        public ResultCode GetFirmwareVersion2(ServiceCtx context)
        {
            // TODO: difference from the command above?

            return GetFirmwareVersionImpl(context);
        }

        private ResultCode GetFirmwareVersionImpl(ServiceCtx context)
        {
            var fwVersionBuf = context.Request.RecvListBuff[0];

            if (!EnsureFirmwareVersionLoaded())
            {
                return ResultCode.NullFirmwareVersionBuffer;
            }

            context.Memory.Write(fwVersionBuf.Position, _firmwareVersion);
            return ResultCode.Success;
        }

        [CommandHipc(7)]
        // GetLockScreenFlag() -> bool
        public ResultCode GetLockScreenFlag(ServiceCtx context)
        {
            context.ResponseData.Write(Horizon.Instance.State.LockScreenFlag);

            return ResultCode.Success;
        }

        [CommandHipc(8)]
        // SetLockScreenFlag(bool)
        public ResultCode SetLockScreenFlag(ServiceCtx context)
        {
            Horizon.Instance.State.LockScreenFlag = context.RequestData.ReadBoolean();

            return ResultCode.Success;
        }

        [CommandHipc(17)]
        // GetAccountSettings() -> nn::settings::system::AccountSettings
        public ResultCode GetAccountSettings(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.AccountSettings);

            return ResultCode.Success;
        }

        [CommandHipc(18)]
        // SetAccountSettings(nn::settings::system::AccountSettings)
        public ResultCode SetAccountSettings(ServiceCtx context)
        {
            Horizon.Instance.State.AccountSettings = context.RequestData.ReadStruct<AccountSettings>();

            return ResultCode.Success;
        }

        [CommandHipc(21)]
        // GetEulaVersions() -> (u32, buffer<nn::settings::system::EulaVersion, 6>)
        public ResultCode GetEulaVersions(ServiceCtx context)
        {
            var eulaVersionsBuf = context.Request.ReceiveBuff[0];

            ulong offset = 0;
            foreach (var eulaVersion in Horizon.Instance.State.EulaVersions)
            {
                context.Memory.Write(eulaVersionsBuf.Position + offset * (ulong)Marshal.SizeOf<EulaVersion>(), eulaVersion);
                offset++;
            }

            context.ResponseData.Write((uint)offset);

            return ResultCode.Success;
        }

        [CommandHipc(22)]
        // SetEulaVersions(buffer<nn::settings::system::EulaVersion, 5>)
        public ResultCode SetEulaVersions(ServiceCtx context)
        {
            var eulaVersionsBuf = context.Request.SendBuff[0];
            var eulaVersionCount = eulaVersionsBuf.Size / (ulong)Marshal.SizeOf<EulaVersion>();

            Horizon.Instance.State.EulaVersions.Clear();
            for (ulong i = 0; i < eulaVersionCount; i++)
            {
                var eulaVersion = context.Memory.Read<EulaVersion>(eulaVersionsBuf.Position + i * (ulong)Marshal.SizeOf<EulaVersion>());
                Horizon.Instance.State.EulaVersions.Add(eulaVersion);
            }

            return ResultCode.Success;
        }

        [CommandHipc(23)]
        // GetColorSetId() -> u32
        public ResultCode GetColorSetId(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.ColorSetId);

            return ResultCode.Success;
        }

        [CommandHipc(24)]
        // SetColorSetId(u32)
        public ResultCode SetColorSetId(ServiceCtx context)
        {
            var colorSetId = context.RequestData.ReadStruct<ColorSetId>();

            Horizon.Instance.State.ColorSetId = colorSetId;

            return ResultCode.Success;
        }

        [CommandHipc(29)]
        // GetNotificationSettings() -> nn::settings::system::NotificationSettings
        public ResultCode GetNotificationSettings(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.NotificationSettings);

            return ResultCode.Success;
        }

        [CommandHipc(31)]
        // GetAccountNotificationSettings() -> (u32, buffer<nn::settings::system::AccountNotificationSettings, 6>)
        public ResultCode GetAccountNotificationSettings(ServiceCtx context)
        {
            var accountNotifSettingsBuf = context.Request.ReceiveBuff[0];

            ulong offset = 0;
            foreach (var accountNotifSettings in Horizon.Instance.State.AccountNotificationSettings)
            {
                context.Memory.Write(accountNotifSettingsBuf.Position + offset * (ulong)Marshal.SizeOf<AccountNotificationSettings>(), accountNotifSettings);
                offset++;
            }

            context.ResponseData.Write((uint)offset);

            return ResultCode.Success;
        }

        [CommandHipc(37)]
        // GetSettingsItemValueSize(buffer<nn::settings::SettingsName, 0x19>, buffer<nn::settings::SettingsItemKey, 0x19>) -> u64
        public ResultCode GetSettingsItemValueSize(ServiceCtx context)
        {
            ulong classPos  = context.Request.PtrBuff[0].Position;
            ulong classSize = context.Request.PtrBuff[0].Size;

            ulong namePos  = context.Request.PtrBuff[1].Position;
            ulong nameSize = context.Request.PtrBuff[1].Size;

            byte[] classBuffer = new byte[classSize];

            context.Memory.Read(classPos, classBuffer);

            byte[] nameBuffer = new byte[nameSize];

            context.Memory.Read(namePos, nameBuffer);

            string askedSetting = Encoding.ASCII.GetString(classBuffer).Trim('\0') + "!" + Encoding.ASCII.GetString(nameBuffer).Trim('\0');

            NxSettings.Settings.TryGetValue(askedSetting, out object nxSetting);

            if (nxSetting != null)
            {
                ulong settingSize;

                if (nxSetting is string stringValue)
                {
                    settingSize = (ulong)stringValue.Length + 1;
                }
                else if (nxSetting is int)
                {
                    settingSize = sizeof(int);
                }
                else if (nxSetting is bool)
                {
                    settingSize = 1;
                }
                else
                {
                    throw new NotImplementedException(nxSetting.GetType().Name);
                }

                context.ResponseData.Write(settingSize);
            }

            return ResultCode.Success;
        }

        [CommandHipc(38)]
        // GetSettingsItemValue(buffer<nn::settings::SettingsName, 0x19, 0x48>, buffer<nn::settings::SettingsItemKey, 0x19, 0x48>) -> (u64, buffer<unknown, 6, 0>)
        public ResultCode GetSettingsItemValue(ServiceCtx context)
        {
            ulong classPos  = context.Request.PtrBuff[0].Position;
            ulong classSize = context.Request.PtrBuff[0].Size;

            ulong namePos  = context.Request.PtrBuff[1].Position;
            ulong nameSize = context.Request.PtrBuff[1].Size;

            ulong replyPos  = context.Request.ReceiveBuff[0].Position;
            ulong replySize = context.Request.ReceiveBuff[0].Size;

            byte[] classBuffer = new byte[classSize];

            context.Memory.Read(classPos, classBuffer);

            byte[] nameBuffer = new byte[nameSize];

            context.Memory.Read(namePos, nameBuffer);

            string askedSetting = Encoding.ASCII.GetString(classBuffer).Trim('\0') + "!" + Encoding.ASCII.GetString(nameBuffer).Trim('\0');

            NxSettings.Settings.TryGetValue(askedSetting, out object nxSetting);

            if (nxSetting != null)
            {
                byte[] settingBuffer = new byte[replySize];

                if (nxSetting is string stringValue)
                {
                    if ((ulong)(stringValue.Length + 1) > replySize)
                    {
                        Logger.Error?.Print(LogClass.ServiceSet, $"{askedSetting} String value size is too big!");
                    }
                    else
                    {
                        settingBuffer = Encoding.ASCII.GetBytes(stringValue + "\0");
                    }
                }

                if (nxSetting is int intValue)
                {
                    settingBuffer = BitConverter.GetBytes(intValue);
                }
                else if (nxSetting is bool boolValue)
                {
                    settingBuffer[0] = boolValue ? (byte)1 : (byte)0;
                }
                else
                {
                    throw new NotImplementedException(nxSetting.GetType().Name);
                }

                context.Memory.Write(replyPos, settingBuffer);

                Logger.Debug?.Print(LogClass.ServiceSet, $"{askedSetting} set value: {nxSetting} as {nxSetting.GetType()}");
            }
            else
            {
                Logger.Error?.Print(LogClass.ServiceSet, $"{askedSetting} not found!");
            }

            return ResultCode.Success;
        }

        [CommandHipc(39)]
        // GetTvSettings() -> nn::settings::system::TvSettings
        public ResultCode GetTvSettings(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.TvSettings);

            return ResultCode.Success;
        }

        [CommandHipc(47)]
        // GetQuestFlag() -> bool
        public ResultCode GetQuestFlag(ServiceCtx context)
        {
            context.ResponseData.Write(Horizon.Instance.State.QuestFlag);

            return ResultCode.Success;
        }

        [CommandHipc(57)]
        // SetRegionCode(nn::settings::RegionCode)
        public ResultCode SetRegionCode(ServiceCtx context)
        {
            var regionCode = context.RequestData.ReadStruct<RegionCode>();

            Horizon.Instance.State.DesiredRegionCode = regionCode;

            return ResultCode.Success;
        }

        [CommandHipc(60)]
        // IsUserSystemClockAutomaticCorrectionEnabled() -> bool
        public ResultCode IsUserSystemClockAutomaticCorrectionEnabled(ServiceCtx context)
        {
            // NOTE: When set to true, is automatically synced with the internet.
            context.ResponseData.Write(Horizon.Instance.State.UserSystemClockAutomaticCorrectionEnabled);

            return ResultCode.Success;
        }

        [CommandHipc(63)]
        // GetPrimaryAlbumStorage() -> nn::settings::system::PrimaryAlbumStorage
        public ResultCode GetPrimaryAlbumStorage(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.PrimaryAlbumStorage);

            return ResultCode.Success;
        }

        [CommandHipc(71)]
        // GetSleepSettings() -> nn::settings::system::SleepSettings
        public ResultCode GetSleepSettings(ServiceCtx context)
        {
            // NOTE: this should be customizable by the user

            context.ResponseData.WriteStruct(Horizon.Instance.State.SleepSettings);

            return ResultCode.Success;
        }

        [CommandHipc(75)]
        // GetInitialLaunchSettings() -> nn::settings::system::InitialLaunchSettings
        public ResultCode GetInitialLaunchSettings(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.InitialLaunchSettings);

            return ResultCode.Success;
        }

        [CommandHipc(76)]
        // SetInitialLaunchSettings(nn::settings::system::InitialLaunchSettings)
        public ResultCode SetInitialLaunchSettings(ServiceCtx context)
        {
            var initialLaunchSettings = context.RequestData.ReadStruct<InitialLaunchSettings>();

            Horizon.Instance.State.InitialLaunchSettings = initialLaunchSettings;

            return ResultCode.Success;
        }

        [CommandHipc(77)]
        // GetDeviceNickName() -> buffer<nn::settings::system::DeviceNickName, 0x16>
        public ResultCode GetDeviceNickName(ServiceCtx context)
        {
            var deviceNickNameBuf = context.Request.ReceiveBuff[0];

            if (deviceNickNameBuf.Size != 0x80)
            {
                Logger.Warning?.Print(LogClass.ServiceSet, "Wrong buffer size");
            }

            // TODO: fill buffer with zeros before writing name?
            context.Memory.Write(deviceNickNameBuf.Position, Encoding.ASCII.GetBytes(Horizon.Instance.State.DeviceNickName + '\0'));

            return ResultCode.Success;
        }

        [CommandHipc(78)]
        // SetDeviceNickName(buffer<nn::settings::system::DeviceNickName, 0x15>)
        public ResultCode SetDeviceNickName(ServiceCtx context)
        {
            var deviceNickNameBuffer = context.Request.SendBuff[0];

            Horizon.Instance.State.DeviceNickName = MemoryHelper.ReadAsciiString(context.Memory, deviceNickNameBuffer.Position, (long)deviceNickNameBuffer.Size);

            return ResultCode.Success;
        }

        [CommandHipc(79)]
        // GetProductModel() -> u32
        public ResultCode GetProductModel(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.ProductModel);

            return ResultCode.Success;
        }

        [CommandHipc(90)]
        // GetMiiAuthorId() -> nn::util::Uuid
        public ResultCode GetMiiAuthorId(ServiceCtx context)
        {
            // NOTE: If miiAuthorId is null ResultCode.NullMiiAuthorIdBuffer is returned.
            //       Doesn't occur in our case.

            context.ResponseData.WriteStruct(Horizon.Instance.State.MiiAuthorId);

            return ResultCode.Success;
        }

        [CommandHipc(95)]
        // GetAutoUpdateEnableFlag() -> bool
        public ResultCode GetAutoUpdateEnableFlag(ServiceCtx context)
        {
            context.ResponseData.Write(Horizon.Instance.State.AutoUpdateEnableFlag);

            return ResultCode.Success;
        }

        [CommandHipc(99)] // 2.0.0+
        // GetBatteryPercentageFlag() -> bool
        public ResultCode GetBatteryPercentageFlag(ServiceCtx context)
        {
            context.ResponseData.Write(Horizon.Instance.State.BatteryPercentageFlag);

            return ResultCode.Success;
        }

        [CommandHipc(124)]
        // GetErrorReportSharePermission() -> nn::settings::system::ErrorReportSharePermission
        public ResultCode GetErrorReportSharePermission(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.ErrorReportSharePermission);

            return ResultCode.Success;
        }

        [CommandHipc(126)]
        // GetAppletLaunchFlags() -> nn::settings::system::AppletLaunchFlag
        public ResultCode GetAppletLaunchFlags(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.AppletLaunchFlags);

            return ResultCode.Success;
        }

        [CommandHipc(136)]
        // GetKeyboardLayout() -> nn::settings::KeyboardLayout
        public ResultCode GetKeyboardLayout(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.DesiredKeyboardLayout);

            return ResultCode.Success;
        }

        [CommandHipc(170)]
        // GetChineseTraditionalInputMethod() -> nn::settings::ChineseTraditionalInputMethod
        public ResultCode GetChineseTraditionalInputMethod(ServiceCtx context)
        {
            context.ResponseData.WriteStruct(Horizon.Instance.State.ChineseTraditionalInputMethod);

            return ResultCode.Success;
        }
    }
}
