using Ryujinx.HLE.HOS.Services.Time.Types;

namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    struct InitialLaunchSettings
    {
        public uint Flags { get; set; }

        public uint Reserved { get; set; }

        public SteadyClockContext TimeStamp { get; set; }

        public static InitialLaunchSettings Default = new InitialLaunchSettings
        {
            Flags = (uint)(InitialLaunchFlag.CompletionFlag | InitialLaunchFlag.UserAdditionFlag | InitialLaunchFlag.TimestampFlag),
            TimeStamp = new SteadyClockContext()
        };
    }
}
