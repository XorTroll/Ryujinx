﻿using Ryujinx.HLE.HOS.Services.Nfc.NfcManager;

namespace Ryujinx.HLE.HOS.Services.Nfc
{
    [Service("nfc:user")]
    class IUserManager : IpcService
    {
        public IUserManager() { }

        [CommandHipc(0)]
        // CreateUserInterface() -> object<nn::nfc::detail::IUser>
        public ResultCode CreateUserInterface(ServiceCtx context)
        {
            MakeObject(context, new INfc(NfcPermissionLevel.User));

            return ResultCode.Success;
        }
    }
}