namespace Ryujinx.HLE.HOS.Services.Settings
{
    public enum RegionCode : int
    {
        Japan,
        USA,
        Europe,
        Australia,
        China,
        Korea,
        Taiwan,

        Min = Japan,
        Max = Taiwan
    }
}
