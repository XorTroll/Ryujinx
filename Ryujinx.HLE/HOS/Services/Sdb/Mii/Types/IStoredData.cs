using System;

namespace Ryujinx.HLE.HOS.Services.Sdb.Mii
{
    interface IStoredData<T> : IElement, IEquatable<T> where T : notnull
    {
        byte Type { get; }

        CreateId CreateId { get; }

        ResultCode InvalidData { get; }

        bool IsValid();
    }
}
