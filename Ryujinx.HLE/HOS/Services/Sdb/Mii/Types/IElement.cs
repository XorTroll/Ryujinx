namespace Ryujinx.HLE.HOS.Services.Sdb.Mii
{
    interface IElement
    {
        void SetFromStoreData(StoreData storeData);

        void SetSource(Source source);
    }
}
