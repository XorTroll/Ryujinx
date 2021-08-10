using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Common;
using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Mouse;

namespace Ryujinx.HLE.HOS.Services.Hid
{
    public class MouseDevice : BaseDevice
    {
        public MouseDevice(bool active) : base(active) { }

        public void Update(int mouseX, int mouseY, uint buttons = 0, int scrollX = 0, int scrollY = 0, bool connected = false)
        {
            Horizon.Instance.Device.Hid.DoForEachSharedMemory((ref SharedMemory.SharedMemory shmem) =>
            {
                ref RingLifo<MouseState> lifo = ref shmem.Mouse;

                ref MouseState previousEntry = ref lifo.GetCurrentEntryRef();

                MouseState newState = new MouseState()
                {
                    SamplingNumber = previousEntry.SamplingNumber + 1,
                };

                if (Active)
                {
                    newState.Buttons = (MouseButton)buttons;
                    newState.X = mouseX;
                    newState.Y = mouseY;
                    newState.DeltaX = mouseX - previousEntry.DeltaX;
                    newState.DeltaY = mouseY - previousEntry.DeltaY;
                    newState.WheelDeltaX = scrollX;
                    newState.WheelDeltaY = scrollY;
                    newState.Attributes = connected ? MouseAttribute.IsConnected : MouseAttribute.None;
                }

                lifo.Write(ref newState);
            });
        }
    }
}