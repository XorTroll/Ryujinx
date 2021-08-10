namespace Ryujinx.HLE.HOS.Services.Hid
{
    enum AppletFooterUiType : byte
    {
        None,
        HandheldNone,
        HandheldJoyConLeftOnly,
        HandheldJoyConRightOnly,
        HandheldJoyConLeftJoyConRight,
        JoyDual,
        JoyDualLeftOnly,
        JoyDualRightOnly,
        JoyLeftHorizontal,
        JoyLeftVertical,
        JoyRightHorizontal,
        JoyRightVertical,
        SwitchProController,
        CompatibleProController,
        CompatibleJoyCon,
        LarkHvc1,
        LarkHvc2,
        LarkNesLeft,
        LarkNesRight,
        Lucia,
        Verification
    }
}