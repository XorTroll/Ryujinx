using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryujinx.HLE.FileSystem.Content
{
    public static class SystemProgramIds
    {
        // TODO: keep adding more

        public static class Applets
        {
            public const ulong Qlaunch = 0x0100000000001000;
            public const ulong OverlayDisp = 0x010000000000100C;
            public const ulong Controller = 0x0100000000001003;
            public const ulong Swkbd = 0x0100000000001008;
            public const ulong MiiEdit = 0x0100000000001009;
            public const ulong Shop = 0x010000000000100B;
            public const ulong PhotoViewer = 0x010000000000100D;
            public const ulong MyPage = 0x0100000000001013;

            public static ulong[] All => new ulong[] { Qlaunch, OverlayDisp, Controller, Swkbd, MiiEdit, Shop, PhotoViewer, MyPage };
        }

        public static class SystemApplications
        {
            public const ulong Starter = 0x0100000000001012;

            public const ulong Flog = 0x01008BB00013C000;

            public const ulong RetailInteractiveDisplayMenu  = 0x0100069000078000;

            public static ulong[] All = new ulong[] { Starter, Flog, RetailInteractiveDisplayMenu };
        }

        public static class SystemArchives
        {
            public const ulong SystemVersion = 0x0100000000000809;
            public const ulong TimeZoneBinary = 0x010000000000080E;
            public const ulong FontNintendoExtension = 0x0100000000000810;
            public const ulong FontStandard = 0x0100000000000811;
            public const ulong FontKorean = 0x0100000000000812;
            public const ulong FontChineseTraditional = 0x0100000000000813;
            public const ulong FontChineseSimple = 0x0100000000000814;
            public const ulong SystemUpdate = 0x0100000000000816;

            public static ulong[] All => new ulong[] { SystemVersion, TimeZoneBinary, FontNintendoExtension, FontStandard, FontKorean, FontChineseTraditional, FontChineseSimple, SystemUpdate };
        }
    }
}
