namespace Ryujinx.HLE.HOS.Services.Settings
{
    public struct NotificationSettings
    {
        public NotificationFlag NotificationFlags { get; set; }

        public NotificationVolume NotificationVolume { get; set; }

        public NotificationTime HeadTime { get; set; }

        public NotificationTime TailTime { get; set; }

        public static NotificationSettings Default = new NotificationSettings
        {
            NotificationFlags = NotificationFlag.RingtoneFlag,
            NotificationVolume = NotificationVolume.Mute,
            HeadTime = new NotificationTime
            {
                Hour = 0,
                Minute = 0
            },
            TailTime = new NotificationTime
            {
                Hour = 0,
                Minute = 0
            }
        };
    }
}
