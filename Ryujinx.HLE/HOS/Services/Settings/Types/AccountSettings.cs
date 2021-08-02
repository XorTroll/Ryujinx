namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    class AccountSettings
    {
        public uint SelectorFlag { get; set; }

        public static AccountSettings Default = new AccountSettings
        {
            SelectorFlag = (uint)UserSelectorFlag.SkipIfSingleUser
        };
    }
}
