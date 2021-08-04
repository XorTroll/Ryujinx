using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryujinx.HLE.HOS.Services.Friend.ServiceCreator
{
    // TODO: guessed fields
    struct PlayHistoryStatistics
    {
        public ulong PlayHistoryLocal { get; set; }

        public ulong PlayHistoryOnline { get; set; }

        public static PlayHistoryStatistics Default = new()
        {
            PlayHistoryLocal = 0,
            PlayHistoryOnline = 0
        };
    }
}
