using Ryujinx.HLE.HOS.Services.Psc.Ovln.Sender;

namespace Ryujinx.HLE.HOS.Services.Psc.Ovln
{
    [Service("ovln:snd")]
    class ISenderService : IpcService
    {
        [CommandHipc(0)]
        // OpenSender() -> object<nn::ovln::ISender>
        public ResultCode OpenSender(ServiceCtx context)
        {
            MakeObject(context, new ISender());

            return ResultCode.Success;
        }
    }
}