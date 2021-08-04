namespace Ryujinx.HLE.HOS.Services.Settings
{
    public enum KeyboardLayout : int
    {
        Default = 0,
        EnglishUs,
        EnglishUsInternational,
        EnglishUk,
        French,
        FrenchCa,
        Spanish,
        SpanishLatin,
        German,
        Italian,
        Portuguese,
        Russian,
        Korean,
        ChineseSimplified,
        ChineseTraditional,

        Min = Default,
        Max = ChineseTraditional
    }
}
