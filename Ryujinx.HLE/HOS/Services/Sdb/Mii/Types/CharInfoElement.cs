﻿using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Sdb.Mii
{
    [StructLayout(LayoutKind.Sequential, Size = 0x5C)]
    struct CharInfoElement : IElement
    {
        public CharInfo CharInfo;
        public Source   Source;

        public void SetFromStoreData(StoreData storeData)
        {
            CharInfo.SetFromStoreData(storeData);
        }

        public void SetSource(Source source)
        {
            Source = source;
        }
    }
}
