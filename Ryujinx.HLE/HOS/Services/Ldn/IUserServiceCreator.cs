using Ryujinx.HLE.HOS.Services.Ldn.UserServiceCreator;

namespace Ryujinx.HLE.HOS.Services.Ldn
{
    [Service("ldn:u")]
    class IUserServiceCreator : IpcService
    {
        [CommandHipc(0)]
        // CreateUserLocalCommunicationService() -> object<nn::ldn::detail::IUserLocalCommunicationService>
        public ResultCode CreateUserLocalCommunicationService(ServiceCtx context)
        {
            MakeObject(context, new IUserLocalCommunicationService());

            return ResultCode.Success;
        }
    }
}