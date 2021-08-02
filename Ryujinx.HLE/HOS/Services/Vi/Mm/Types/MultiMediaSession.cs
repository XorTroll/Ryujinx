namespace Ryujinx.HLE.HOS.Services.Vi.Mm
{
    class MultiMediaSession
    {
        public MultiMediaOperationType Type { get; }

        public bool IsAutoClearEvent { get; }
        public uint Id               { get; }
        public uint CurrentValue     { get; private set; }

        public MultiMediaSession(uint id, MultiMediaOperationType type, bool isAutoClearEvent)
        {
            Type             = type;
            Id               = id;
            IsAutoClearEvent = isAutoClearEvent;
            CurrentValue     = 0;
        }

        public void SetAndWait(uint value, int timeout)
        {
            CurrentValue = value;
        }
    }
}
