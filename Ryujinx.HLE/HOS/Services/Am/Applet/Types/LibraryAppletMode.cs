using System;

namespace Ryujinx.HLE.HOS.Services.Am.Applet
{
    [Flags]
    enum LibraryAppletMode : uint
    {
        AllForeground,
        PartialForeground,
        NoUi,
        PartialForegroundWithIndirectDisplay,
        AllForegroundInitiallyHidden
    }
}