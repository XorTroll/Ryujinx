using System;
using System.Collections.Concurrent;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    interface IAppletFifo<T> : IProducerConsumerCollection<T>
    {
        event EventHandler DataAvailable;
    }
}
