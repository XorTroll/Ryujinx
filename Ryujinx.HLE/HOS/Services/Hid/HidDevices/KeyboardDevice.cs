using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Common;
using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Keyboard;
using System;

namespace Ryujinx.HLE.HOS.Services.Hid
{
    public class KeyboardDevice : BaseDevice
    {
        public KeyboardDevice(bool active) : base(active) { }

        public unsafe void Update(KeyboardInput keyState)
        {
            ref RingLifo<KeyboardState> lifo = ref Horizon.Instance.Device.Hid.SharedMemory.Keyboard;

            if (!Active)
            {
                lifo.Clear();

                return;
            }

            ref KeyboardState previousEntry = ref lifo.GetCurrentEntryRef();

            KeyboardState newState = new KeyboardState
            {
                SamplingNumber = previousEntry.SamplingNumber + 1,
            };

            keyState.Keys.AsSpan().CopyTo(newState.Keys.RawData.ToSpan());
            newState.Modifiers = (KeyboardModifier)keyState.Modifier;

            lifo.Write(ref newState);
        }
    }
}