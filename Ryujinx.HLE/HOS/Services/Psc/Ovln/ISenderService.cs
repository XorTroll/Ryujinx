using Ryujinx.HLE.HOS.Services.Psc.Ovln.Sender;

namespace Ryujinx.HLE.HOS.Services.Psc.Ovln
{
    [Service("ovln:snd")]
    class ISenderService : IpcService
    {
        public ISenderService() { }

        [CommandHipc(0)]
        // OpenSender() -> object<nn::ovln::ISender>
        public ResultCode OpenSender(ServiceCtx context)
        {
            MakeObject(context, new ISender(context));

            return ResultCode.Success;
        }
    }
}