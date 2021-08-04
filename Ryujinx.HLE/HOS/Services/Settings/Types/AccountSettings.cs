namespace Ryujinx.HLE.HOS.Services.Settings
{
    public struct AccountSettings
    {
        public UserSelectorFlag UserSelectorFlags { get; set; }

        public static AccountSettings Default = new AccountSettings
        {
            UserSelectorFlags = UserSelectorFlag.SkipIfSingleUser
        };
    }
}
