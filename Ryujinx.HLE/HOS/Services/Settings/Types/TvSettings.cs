namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    class TvSettings
    {
        public uint Flags { get; set; }

        public uint Resolution { get; set; }

        public uint HdmiCntType { get; set; }

        public uint Rgb { get; set; }

        public uint Cmu { get; set; }

        public uint Underscan { get; set; }

        public float Gamma { get; set; }

        public uint ContrastRatio { get; set; }

        public static TvSettings Default = new TvSettings
        {
            Flags = (uint)TvFlag.Allows4k,
            Resolution = (uint)TvResolution.R_1080p,
            HdmiCntType = (uint)HdmiContentType.Game,
            Rgb = (uint)RgbRange.Full,
            Cmu = (uint)CmuMode.None,
            Underscan = 0,
            Gamma = 2.2f,
            ContrastRatio = 10
        };
    }
}
