using Ryujinx.HLE.HOS.Services.Settings;
using Ryujinx.HLE.Utilities;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.SystemState
{
    public class SystemStateMgr
    {
        internal static string[] LanguageCodes = new string[]
        {
            "ja",
            "en-US",
            "fr",
            "de",
            "it",
            "es",
            "zh-CN",
            "ko",
            "nl",
            "pt",
            "ru",
            "zh-TW",
            "en-GB",
            "fr-CA",
            "es-419",
            "zh-Hans",
            "zh-Hant"
        };

        // TODO: make customizable (from the UI) all fields here (except those which can be edited using qlaunch?)

        public KeyboardLayout DesiredKeyboardLayout { get; set; }

        public SystemLanguage DesiredSystemLanguage { get; set; }

        public long DesiredLanguageCode { get; set; }

        public RegionCode DesiredRegionCode { get; set; }

        public TitleLanguage DesiredTitleLanguage { get; set; }

        public bool DockedMode { get; set; }

        public ColorSetId ColorSetId { get; set; }

        public string DeviceNickName { get; set; }

        public bool LockScreenFlag { get; set; }

        public AccountSettings AccountSettings { get; set; }

        public List<EulaVersion> EulaVersions { get; set; }

        public NotificationSettings NotificationSettings { get; set; }

        public List<AccountNotificationSettings> AccountNotificationSettings { get; set; }

        public TvSettings TvSettings { get; set; }

        public bool QuestFlag { get; set; }

        public bool UserSystemClockAutomaticCorrectionEnabled { get; set; }

        public PrimaryAlbumStorage PrimaryAlbumStorage { get; set; }

        public SleepSettings SleepSettings { get; set; }

        public InitialLaunchSettings InitialLaunchSettings { get; set; }

        public ProductModel ProductModel { get; set; }

        public UInt128 MiiAuthorId { get; set; }

        public bool AutoUpdateEnableFlag { get; set; }

        public bool BatteryPercentageFlag { get; set; }

        public ErrorReportSharePermission ErrorReportSharePermission { get; set; }

        public AppletLaunchFlag AppletLaunchFlags { get; set; }

        public ChineseTraditionalInputMethod ChineseTraditionalInputMethod { get; set; }

        public PlatformRegion PlatformRegion => (DesiredRegionCode == RegionCode.China) ? PlatformRegion.China : PlatformRegion.Global;

        public SystemStateMgr()
        {
            // TODO: Let user specify fields.
            DesiredKeyboardLayout = (long)KeyboardLayout.Default;
            DeviceNickName        = "Ryujinx's Switch";
        }

        public void SetLanguage(SystemLanguage language)
        {
            DesiredSystemLanguage = language;
            DesiredLanguageCode   = GetLanguageCode((int)DesiredSystemLanguage);

            DesiredTitleLanguage = language switch
            {
                SystemLanguage.Taiwanese or
                SystemLanguage.TraditionalChinese => TitleLanguage.Taiwanese,
                SystemLanguage.Chinese or
                SystemLanguage.SimplifiedChinese  => TitleLanguage.Chinese,
                _                                 => Enum.Parse<TitleLanguage>(Enum.GetName(typeof(SystemLanguage), language)),
            };
        }

        public void SetRegion(RegionCode region)
        {
            DesiredRegionCode = region;
        }

        internal static long GetLanguageCode(int index)
        {
            if ((uint)index >= LanguageCodes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            long code  = 0;
            int  shift = 0;

            foreach (char chr in LanguageCodes[index])
            {
                code |= (long)(byte)chr << shift++ * 8;
            }

            return code;
        }
    }
}