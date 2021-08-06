using System;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    [Flags]
    public enum LibraryAppletMode : uint
    {
        AllForeground,
        PartialForeground,
        NoUi,
        PartialForegroundWithIndirectDisplay,
        AllForegroundInitiallyHidden
    }
}