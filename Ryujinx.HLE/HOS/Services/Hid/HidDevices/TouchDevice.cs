using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Common;
using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.TouchScreen;
using System;

namespace Ryujinx.HLE.HOS.Services.Hid
{
    public class TouchDevice : BaseDevice
    {
        public TouchDevice(bool active) : base(active) { }

        public void Update(params TouchPoint[] points)
        {
            Horizon.Instance.Device.Hid.DoForEachSharedMemory((ref SharedMemory.SharedMemory shmem) =>
            {
                ref RingLifo<TouchScreenState> lifo = ref shmem.TouchScreen;

                ref TouchScreenState previousEntry = ref lifo.GetCurrentEntryRef();

                TouchScreenState newState = new TouchScreenState
                {
                    SamplingNumber = previousEntry.SamplingNumber + 1
                };

                if (Active)
                {
                    newState.TouchesCount = points.Length;

                    int pointsLength = Math.Min(points.Length, newState.Touches.Length);

                    for (int i = 0; i < pointsLength; ++i)
                    {
                        TouchPoint pi = points[i];
                        newState.Touches[i] = new TouchState
                        {
                            DeltaTime = newState.SamplingNumber,
                            Attribute = pi.Attribute,
                            X = pi.X,
                            Y = pi.Y,
                            FingerId = (uint)i,
                            DiameterX = pi.DiameterX,
                            DiameterY = pi.DiameterY,
                            RotationAngle = pi.Angle
                        };
                    }
                }

                lifo.Write(ref newState);
            });
        }
    }
}