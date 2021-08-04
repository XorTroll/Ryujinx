using static Ryujinx.HLE.HOS.Services.Hid.Hid;

namespace Ryujinx.HLE.HOS.Services.Hid
{
    public abstract class BaseDevice
    {
        public bool Active;

        public BaseDevice(bool active)
        {
            Active = active;
        }
    }
}