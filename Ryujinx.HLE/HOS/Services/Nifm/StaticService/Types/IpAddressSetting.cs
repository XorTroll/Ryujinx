﻿using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Nifm.StaticService
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0xd)]
    struct IpAddressSetting
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool        IsDhcpEnabled;
        public IpV4Address Address;
        public IpV4Address IPv4Mask;
        public IpV4Address GatewayAddress;

        public IpAddressSetting(IPInterfaceProperties interfaceProperties, UnicastIPAddressInformation unicastIPAddressInformation)
        {
            IsDhcpEnabled  = interfaceProperties.DhcpServerAddresses.Count != 0;
            Address        = new IpV4Address(unicastIPAddressInformation.Address);
            IPv4Mask       = new IpV4Address(unicastIPAddressInformation.IPv4Mask);
            GatewayAddress = new IpV4Address(interfaceProperties.GatewayAddresses[0].Address);
        }
    }
}
