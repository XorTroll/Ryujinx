﻿namespace Ryujinx.HLE.HOS.Services.Sdb.Mii
{
    class DatabaseSessionMetadata
    {
        public uint  InterfaceVersion;
        public ulong UpdateCounter;

        public SpecialMiiKeyCode MiiKeyCode { get; private set; }

        public DatabaseSessionMetadata(ulong updateCounter, SpecialMiiKeyCode miiKeyCode)
        {
            InterfaceVersion = 0;
            UpdateCounter    = updateCounter;
            MiiKeyCode       = miiKeyCode;
        }

        public bool IsInterfaceVersionSupported(uint interfaceVersion)
        {
            return InterfaceVersion >= interfaceVersion;
        }
    }
}
