using System;

namespace Ryujinx.HLE.Loaders.Npdm
{
    [Flags]
    public enum FsAccessFlag : long
    {
        ApplicationInfo = 1 << 0,
        BootModeControl = 1 << 1,
        Calibration = 1 << 2,
        SystemSaveData = 1 << 3,
        GameCard = 1 << 4,
        SaveDataBackup = 1 << 5,
        SaveDataManagement = 1 << 6,
        BisAllRaw = 1 << 7,
        GameCardRaw = 1 << 8,
        GameCardPrivate = 1 << 9,
        SetTime = 1 << 10,
        ContentManager = 1 << 11,
        ImageManager = 1 << 12,
        CreateSaveData = 1 << 13,
        SystemSaveDataManagement = 1 << 14,
        BisFileSystem = 1 << 15,
        SystemUpdate = 1 << 16,
        SaveDataMeta = 1 << 17,
        DeviceSaveData = 1 << 18,
        SettingsControl = 1 << 19,
        SystemData = 1 << 20,
        SdCard = 1 << 21,
        Host = 1 << 22,
        FillBis = 1 << 23,
        CorruptSaveData = 1 << 24,
        SaveDataForDebug = 1 << 25,
        FormatSdCard = 1 << 26,
        GetRightsId = 1 << 27,
        RegisterExternalKey = 1 << 28,
        RegisterUpdatePartition = 1 << 29,
        SaveDataTransfer = 1 << 30,
        DeviceDetection = 1 << 31,
        AccessFailureResolution = 1 << 32,
        SaveDataTransferV2 = 1 << 33,
        RegisterProgramIndexMapInfo = 1 << 34,
        CreateOwnSaveData = 1 << 35,
        MoveCacheStorage = 1 << 36,
        Debug = 1 << 62,
        FullPermission = 1 << 63
    }
}
