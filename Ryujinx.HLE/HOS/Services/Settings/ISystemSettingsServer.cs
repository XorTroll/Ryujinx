using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;
using Ryujinx.Common;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.HOS.SystemState;
using Ryujinx.HLE.Utilities;
using Ryujinx.HLE.HOS.Services.Sdb.Mii;
using Ryujinx.HLE.HOS.Services.Settings.Types;
using System;
using System.IO;
using System.Text;

namespace Ryujinx.HLE.HOS.Services.Settings
{
    [Service("set:sys")]
    class ISystemSettingsServer : IpcService
    {
        public ISystemSettingsServer() { }

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
            return GetFirmwareVersion2(context);
        }

        [CommandHipc(4)]
        // GetFirmwareVersion2() -> buffer<nn::settings::system::FirmwareVersion, 0x1a, 0x100>
        public ResultCode GetFirmwareVersion2(ServiceCtx context)
        {
            ulong replyPos  = context.Request.RecvListBuff[0].Position;

            context.Response.PtrBuff[0] = context.Response.PtrBuff[0].WithSize(0x100L);

            byte[] firmwareData = GetFirmwareData(context.Device);

            if (firmwareData != null)
            {
                context.Memory.Write(replyPos, firmwareData);

                return ResultCode.Success;
            }

            const byte majorFwVersion = 0x03;
            const byte minorFwVersion = 0x00;
            const byte microFwVersion = 0x00;
            const byte unknown        = 0x00; //Build?

            const int revisionNumber = 0x0A;

            const string platform   = "NX";
            const string unknownHex = "7fbde2b0bba4d14107bf836e4643043d9f6c8e47";
            const string version    = "3.0.0";
            const string build      = "NintendoSDK Firmware for NX 3.0.0-10.0";

            // http://switchbrew.org/index.php?title=System_Version_Title
            using (MemoryStream ms = new MemoryStream(0x100))
            {
                BinaryWriter writer = new BinaryWriter(ms);

                writer.Write(majorFwVersion);
                writer.Write(minorFwVersion);
                writer.Write(microFwVersion);
                writer.Write(unknown);

                writer.Write(revisionNumber);

                writer.Write(Encoding.ASCII.GetBytes(platform));

                ms.Seek(0x28, SeekOrigin.Begin);

                writer.Write(Encoding.ASCII.GetBytes(unknownHex));

                ms.Seek(0x68, SeekOrigin.Begin);

                writer.Write(Encoding.ASCII.GetBytes(version));

                ms.Seek(0x80, SeekOrigin.Begin);

                writer.Write(Encoding.ASCII.GetBytes(build));

                context.Memory.Write(replyPos, ms.ToArray());
            }

            return ResultCode.Success;
        }

        [CommandHipc(7)]
        // GetLockScreenFlag() -> bool
        public ResultCode GetLockScreenFlag(ServiceCtx context)
        {
            // Note: this should be customizable

            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(17)]
        // GetAccountSettings() -> nn::settings::system::AccountSettings
        public ResultCode GetAccountSettings(ServiceCtx context)
        {
            // Note: this should be customizable
            context.ResponseData.Write(AccountSettings.Default.SelectorFlag);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(21)]
        // GetEulaVersions() -> u32, buffer
        public ResultCode GetEulaVersions(ServiceCtx context)
        {
            // TODO

            var eulaVerBuf = context.Request.ReceiveBuff[0];

            context.Memory.Write(eulaVerBuf.Position, EulaVersion.Default);
            context.ResponseData.Write((uint)1);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(22)]
        // SetEulaVersions(buffer)
        public ResultCode SetEulaVersions(ServiceCtx context)
        {
            // TODO: set

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(23)]
        // GetColorSetId() -> i32
        public ResultCode GetColorSetId(ServiceCtx context)
        {
            // context.ResponseData.Write((int)context.Device.System.State.ThemeColor);
            context.ResponseData.Write((int)ColorSet.BasicBlack);

            return ResultCode.Success;
        }

        [CommandHipc(24)]
        // SetColorSetId() -> i32
        public ResultCode SetColorSetId(ServiceCtx context)
        {
            int colorSetId = context.RequestData.ReadInt32();

            context.Device.System.State.ThemeColor = (ColorSet)colorSetId;

            return ResultCode.Success;
        }

        [CommandHipc(29)]
        // GetNotificationSettings() -> nn::settings::system::NotificationSettings
        public ResultCode GetNotificationSettings(ServiceCtx context)
        {
            context.ResponseData.Write(NotificationSettings.Default.Flags);
            context.ResponseData.Write(NotificationSettings.Default.Volume);
            context.ResponseData.Write(NotificationSettings.Default.HeadTime.Hour);
            context.ResponseData.Write(NotificationSettings.Default.HeadTime.Minute);
            context.ResponseData.Write(NotificationSettings.Default.TailTime.Hour);
            context.ResponseData.Write(NotificationSettings.Default.TailTime.Minute);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(31)]
        // GetAccountNotificationSettings() -> nn::settings::system::AccountNotificationSettings
        public ResultCode GetAccountNotificationSettings(ServiceCtx context)
        {
            context.ResponseData.Write((uint)1);

            var account_notif_settings_buf = context.Request.ReceiveBuff[0];
            var account_notif_settings = AccountNotificationSettings.MakeDefault(context.Device.System.AccountManager.LastOpenedUser.UserId);
            context.Memory.Write(account_notif_settings_buf.Position, account_notif_settings);

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
            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            context.ResponseData.Write(TvSettings.Default.Flags);
            context.ResponseData.Write(TvSettings.Default.Resolution);
            context.ResponseData.Write(TvSettings.Default.HdmiCntType);
            context.ResponseData.Write(TvSettings.Default.Rgb);
            context.ResponseData.Write(TvSettings.Default.Cmu);
            context.ResponseData.Write(TvSettings.Default.Underscan);
            context.ResponseData.Write(TvSettings.Default.Gamma);
            context.ResponseData.Write(TvSettings.Default.ContrastRatio);

            return ResultCode.Success;
        }

        [CommandHipc(47)]
        // GetQuestFlag() -> bool
        public ResultCode GetQuestFlag(ServiceCtx context)
        {
            // We're not Quest (a KIOSK unit)
            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(57)]
        // SetRegionCode(nn::settings::tRegionCode)
        public ResultCode SetRegionCode(ServiceCtx context)
        {
            // TODO: set

            var regionCode = context.RequestData.ReadUInt32();

            Logger.Stub?.PrintStub(LogClass.ServiceSet, new { regionCode });

            return ResultCode.Success;
        }

        [CommandHipc(60)]
        // IsUserSystemClockAutomaticCorrectionEnabled() -> bool
        public ResultCode IsUserSystemClockAutomaticCorrectionEnabled(ServiceCtx context)
        {
            // NOTE: When set to true, is automatically synced with the internet.
            context.ResponseData.Write(true);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(63)]
        // GetPrimaryAlbumStorage() -> nn::settings::system::PrimaryAlbumStorage
        public ResultCode GetPrimaryAlbumStorage(ServiceCtx context)
        {
            // NOTE: THis should be customizable
            // Default to SD card
            context.ResponseData.Write((uint)PrimaryAlbumStorage.SdCard);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(71)]
        // GetSleepSettings() -> nn::settings::system::SleepSettings
        public ResultCode GetSleepSettings(ServiceCtx context)
        {
            // TODO: do this nicely
            uint flag = 0;
            uint handheld_sleep_plan = 5; // Never sleep
            uint console_sleep_plan = 5; // Never sleep
            // NOTE: this should be customizable by the user

            context.ResponseData.Write(flag);
            context.ResponseData.Write(handheld_sleep_plan);
            context.ResponseData.Write(console_sleep_plan);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(75)]
        // GetInitialLaunchSettings() -> nn::settings::system::InitialLaunchSettings
        public ResultCode GetInitialLaunchSettings(ServiceCtx context)
        {
            // NOTE: this really should be customizable, empty on first launch, etc.

            context.ResponseData.Write(InitialLaunchSettings.Default.Flags);
            context.ResponseData.Write(InitialLaunchSettings.Default.Reserved);
            context.ResponseData.Write(InitialLaunchSettings.Default.TimeStamp.InternalOffset);
            context.ResponseData.Write(InitialLaunchSettings.Default.TimeStamp.ClockSourceId.Low);
            context.ResponseData.Write(InitialLaunchSettings.Default.TimeStamp.ClockSourceId.High);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(76)]
        // SetInitialLaunchSettings(nn::settings::system::InitialLaunchSettings)
        public ResultCode SetInitialLaunchSettings(ServiceCtx context)
        {
            // TODO: make this customizable ^

            var initialLaunchSettings = context.RequestData.ReadStruct<InitialLaunchSettings>();

            Logger.Stub?.PrintStub(LogClass.ServiceSet, new { initialLaunchSettings });

            return ResultCode.Success;
        }

        [CommandHipc(77)]
        // GetDeviceNickName() -> buffer<nn::settings::system::DeviceNickName, 0x16>
        public ResultCode GetDeviceNickName(ServiceCtx context)
        {
            ulong deviceNickNameBufferPosition = context.Request.ReceiveBuff[0].Position;
            ulong deviceNickNameBufferSize     = context.Request.ReceiveBuff[0].Size;

            if (deviceNickNameBufferPosition == 0)
            {
                return ResultCode.NullDeviceNicknameBuffer;
            }

            if (deviceNickNameBufferSize != 0x80)
            {
                Logger.Warning?.Print(LogClass.ServiceSet, "Wrong buffer size");
            }

            context.Memory.Write(deviceNickNameBufferPosition, Encoding.ASCII.GetBytes(context.Device.System.State.DeviceNickName + '\0'));

            return ResultCode.Success;
        }

        [CommandHipc(78)]
        // SetDeviceNickName(buffer<nn::settings::system::DeviceNickName, 0x15>)
        public ResultCode SetDeviceNickName(ServiceCtx context)
        {
            ulong deviceNickNameBufferPosition = context.Request.SendBuff[0].Position;
            ulong deviceNickNameBufferSize     = context.Request.SendBuff[0].Size;

            byte[] deviceNickNameBuffer = new byte[deviceNickNameBufferSize];

            context.Memory.Read(deviceNickNameBufferPosition, deviceNickNameBuffer);

            context.Device.System.State.DeviceNickName = Encoding.ASCII.GetString(deviceNickNameBuffer);

            return ResultCode.Success;
        }

        [CommandHipc(79)]
        // GetProductModel() -> u32
        public ResultCode GetProductModel(ServiceCtx context)
        {
            // TODO: make this customizable?

            context.ResponseData.Write((uint)ProductModel.Nx);

            return ResultCode.Success;
        }

        [CommandHipc(90)]
        // GetMiiAuthorId() -> nn::util::Uuid
        public ResultCode GetMiiAuthorId(ServiceCtx context)
        {
            // NOTE: If miiAuthorId is null ResultCode.NullMiiAuthorIdBuffer is returned.
            //       Doesn't occur in our case.

            UInt128 miiAuthorId = Helper.GetDeviceId();

            miiAuthorId.Write(context.ResponseData);

            return ResultCode.Success;
        }

        [CommandHipc(95)]
        // GetAutoUpdateEnableFlag() -> bool
        public ResultCode GetAutoUpdateEnableFlag(ServiceCtx context)
        {
            // Note: this should be customizable

            context.ResponseData.Write(false);

            return ResultCode.Success;
        }

        [CommandHipc(99)] // 2.0.0+
        // GetBatteryPercentageFlag() -> bool
        public ResultCode GetBatteryPercentageFlag(ServiceCtx context)
        {
            // NOTE: This should be customizable by the user (whether to show battery with or without the percentage value)

            context.ResponseData.Write(true);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        public byte[] GetFirmwareData(Switch device)
        {
            const ulong SystemVersionTitleId = 0x0100000000000809;

            string contentPath = device.System.ContentManager.GetInstalledContentPath(SystemVersionTitleId, StorageId.NandSystem, NcaContentType.Data);

            if (string.IsNullOrWhiteSpace(contentPath))
            {
                return null;
            }

            string firmwareTitlePath = device.FileSystem.SwitchPathToSystemPath(contentPath);

            using(IStorage firmwareStorage = new LocalStorage(firmwareTitlePath, FileAccess.Read))
            {
                Nca firmwareContent = new Nca(device.System.KeySet, firmwareStorage);

                if (!firmwareContent.CanOpenSection(NcaSectionType.Data))
                {
                    return null;
                }

                IFileSystem firmwareRomFs = firmwareContent.OpenFileSystem(NcaSectionType.Data, device.System.FsIntegrityCheckLevel);

                Result result = firmwareRomFs.OpenFile(out IFile firmwareFile, "/file".ToU8Span(), OpenMode.Read);
                if (result.IsFailure())
                {
                    return null;
                }

                result = firmwareFile.GetSize(out long fileSize);
                if (result.IsFailure())
                {
                    return null;
                }

                byte[] data = new byte[fileSize];

                result = firmwareFile.Read(out _, 0, data);
                if (result.IsFailure())
                {
                    return null;
                }

                return data;
            }
        }

        [CommandHipc(124)]
        // GetErrorReportSharePermission() -> nn::settings::system::ErrorReportSharePermission
        public ResultCode GetErrorReportSharePermission(ServiceCtx context)
        {
            context.ResponseData.Write((uint)ErrorReportSharePermission.Denied);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(126)]
        // GetAppletLaunchFlags() -> nn::settings::system::AppletLaunchFlag
        public ResultCode GetAppletLaunchFlags(ServiceCtx context)
        {
            // NOTE: unknown values
            uint applet_launch_flag = 0;
            context.ResponseData.Write(applet_launch_flag);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }

        [CommandHipc(136)]
        // GetKeyboardLayout() -> nn::settings::KeyboardLayout
        public ResultCode GetKeyboardLayout(ServiceCtx context)
        {
            context.ResponseData.Write((uint)context.Device.System.State.DesiredKeyboardLayout);

            return ResultCode.Success;
        }

        [CommandHipc(170)]
        // GetChineseTraditionalInputMethod() -> nn::settings::ChineseTraditionalInputMethod
        public ResultCode GetChineseTraditionalInputMethod(ServiceCtx context)
        {
            // TODO
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.ServiceSet);

            return ResultCode.Success;
        }
    }
}
