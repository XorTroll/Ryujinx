﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryujinx.HLE.HOS.Services.Settings.Types
{
    [Flags]
    enum SleepFlag
    {
        SleepsWhilePlayingMedia = 1 << 0,
        WakesAtPowerStateChange = 1 << 1
    }
}
