using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Common;

namespace Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Keyboard
{
    struct KeyboardState : ISampledData
    {
        public ulong SamplingNumber;
        public KeyboardModifier Modifiers;
        public KeyboardKey Keys;

        ulong ISampledData.SamplingNumber => SamplingNumber;
    }
}
