using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Sdb.Mii
{
    [StructLayout(LayoutKind.Sequential, Size = 0x48)]
    struct StoreDataElement : IElement
    {
        public StoreData StoreData;
        public Source    Source;

        public void SetFromStoreData(StoreData storeData)
        {
            StoreData = storeData;
        }

        public void SetSource(Source source)
        {
            Source = source;
        }
    }
}
