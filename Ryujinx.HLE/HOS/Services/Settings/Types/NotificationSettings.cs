namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    class NotificationSettings
    {
        public uint Flags { get; set; }

        public uint Volume { get; set; }

        public NotificationTime HeadTime { get; set; }

        public NotificationTime TailTime { get; set; }

        public static NotificationSettings Default = new NotificationSettings
        {
            Flags = (uint)NotificationFlag.RingtoneFlag,
            Volume = (uint)NotificationVolume.Mute,
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
