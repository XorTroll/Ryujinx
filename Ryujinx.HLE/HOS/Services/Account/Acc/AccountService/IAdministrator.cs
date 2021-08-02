using Ryujinx.Common.Logging;

namespace Ryujinx.HLE.HOS.Services.Account.Acc.AccountService
{
    class IAdministrator : IpcService
    {
        public IAdministrator(UserId userId) { }

        [CommandHipc(0)]
        // CheckAvailability()
        public ResultCode CheckAvailability(ServiceCtx context)
        {
            // TODO

            Logger.Stub?.PrintStub(LogClass.ServiceAcc);

            return ResultCode.Success;
        }

        [CommandHipc(250)]
        // IsLinkedWithNintendoAccount() -> bool
        public ResultCode IsLinkedWithNintendoAccount(ServiceCtx context)
        {
            // TODO

            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.ServiceAcc);

            return ResultCode.Success;
        }
    }
}
