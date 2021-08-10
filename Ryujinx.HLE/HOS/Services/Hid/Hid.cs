using Ryujinx.Common;
using Ryujinx.HLE.Exceptions;
using Ryujinx.Common.Configuration.Hid;
using Ryujinx.Common.Memory;
using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Common;
using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Mouse;
using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Keyboard;
using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.DebugPad;
using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.TouchScreen;
using Ryujinx.HLE.HOS.Services.Hid.SharedMemory.Npad;
using Ryujinx.HLE.HOS.Kernel.Memory;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Shmem = Ryujinx.HLE.HOS.Services.Hid.SharedMemory.SharedMemory;

namespace Ryujinx.HLE.HOS.Services.Hid
{
    public class Hid
    {
        private object _storagesLock;

        private readonly List<SharedMemoryStorage> _storages;

        internal delegate void ShmemAction(ref Shmem shmem);

        internal void DoForEachSharedMemory(ShmemAction fn)
        {
            lock (_storagesLock)
            {
                foreach (var storage in _storages)
                {
                    ref var shmem = ref storage.GetRef<Shmem>(0);
                    fn.Invoke(ref shmem);
                }
            }
        }

        internal const int SharedMemEntryCount = 17;

        public DebugPadDevice DebugPad;
        public TouchDevice    Touchscreen;
        public MouseDevice    Mouse;
        public KeyboardDevice Keyboard;
        public NpadDevices    Npads;

        private static void CheckTypeSizeOrThrow<T>(int expectedSize)
        {
            if (Unsafe.SizeOf<T>() != expectedSize)
            {
                throw new InvalidStructLayoutException<T>(expectedSize);
            }
        }

        static Hid()
        {
            CheckTypeSizeOrThrow<RingLifo<DebugPadState>>(0x2c8);
            CheckTypeSizeOrThrow<RingLifo<TouchScreenState>>(0x2C38);
            CheckTypeSizeOrThrow<RingLifo<MouseState>>(0x350);
            CheckTypeSizeOrThrow<RingLifo<KeyboardState>>(0x3D8);
            CheckTypeSizeOrThrow<Array10<NpadState>>(0x32000);
            CheckTypeSizeOrThrow<Shmem>(Horizon.HidSize);
        }

        internal Hid()
        {
            _storages = new();
            _storagesLock = new object();
        }

        internal void RegisterSharedMemory(SharedMemoryStorage storage)
        {
            lock (_storagesLock)
            {
                _storages.Add(storage);
            }
        }

        internal void RemoveSharedMemory(SharedMemoryStorage storage)
        {
            lock (_storagesLock)
            {
                _storages.Remove(storage);
            }
        }

        public void InitDevices()
        {
            DebugPad    = new DebugPadDevice(true);
            Touchscreen = new TouchDevice(true);
            Mouse       = new MouseDevice(false);
            Keyboard    = new KeyboardDevice(false);
            Npads       = new NpadDevices(true);
        }

        public void RefreshInputConfig(List<InputConfig> inputConfig)
        {
            ControllerConfig[] npadConfig = new ControllerConfig[inputConfig.Count];

            for (int i = 0; i < npadConfig.Length; ++i)
            {
                npadConfig[i].Player = (PlayerIndex)inputConfig[i].PlayerIndex;
                npadConfig[i].Type = (ControllerType)inputConfig[i].ControllerType;
            }

            Horizon.Instance.Device.Hid.Npads.Configure(npadConfig);
        }

        public ControllerKeys UpdateStickButtons(JoystickPosition leftStick, JoystickPosition rightStick)
        {
            const int stickButtonThreshold = short.MaxValue / 2;
            ControllerKeys result = 0;

            result |= (leftStick.Dx < -stickButtonThreshold) ? ControllerKeys.LStickLeft  : result;
            result |= (leftStick.Dx > stickButtonThreshold)  ? ControllerKeys.LStickRight : result;
            result |= (leftStick.Dy < -stickButtonThreshold) ? ControllerKeys.LStickDown  : result;
            result |= (leftStick.Dy > stickButtonThreshold)  ? ControllerKeys.LStickUp    : result;

            result |= (rightStick.Dx < -stickButtonThreshold) ? ControllerKeys.RStickLeft  : result;
            result |= (rightStick.Dx > stickButtonThreshold)  ? ControllerKeys.RStickRight : result;
            result |= (rightStick.Dy < -stickButtonThreshold) ? ControllerKeys.RStickDown  : result;
            result |= (rightStick.Dy > stickButtonThreshold)  ? ControllerKeys.RStickUp    : result;

            return result;
        }

        internal static ulong GetTimestampTicks()
        {
            return (ulong)PerformanceCounter.ElapsedMilliseconds * 19200;
        }
    }
}
