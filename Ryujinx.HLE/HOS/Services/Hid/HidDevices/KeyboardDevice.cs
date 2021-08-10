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
            Horizon.Instance.Device.Hid.DoForEachSharedMemory((ref SharedMemory.SharedMemory shmem) =>
            {
                ref RingLifo<KeyboardState> lifo = ref shmem.Keyboard;

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
            });
        }
    }
}