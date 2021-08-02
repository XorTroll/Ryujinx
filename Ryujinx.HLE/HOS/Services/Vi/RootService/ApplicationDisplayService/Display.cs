namespace Ryujinx.HLE.HOS.Services.Vi
{
    class Display
    {
        public string Name { get; private set; }

        public Display(string name)
        {
            Name = name;
        }
    }

    class DisplayModeInfo
    {
        public uint Width { get; set; }

        public uint Height { get; set; }

        public float Unk { get; set; }

        public uint Unk2 { get; set; }

        public static DisplayModeInfo DefaultDisplayMode = new DisplayModeInfo
        {
            Width = 1280,
            Height = 720,
            Unk = 60.0f,
            Unk2 = 0
        };
    }
}