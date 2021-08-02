using Ryujinx.HLE.HOS.Services.Psc.Ovln.Receiver;

namespace Ryujinx.HLE.HOS.Services.Psc.Ovln
{
    [Service("ovln:rcv")]
    class IReceiverService : IpcService
    {
        public IReceiverService() { }

        [CommandHipc(0)]
        // OpenReceiver() -> object<nn::ovln::IReceiver>
        public ResultCode OpenReceiver(ServiceCtx context)
        {
            // Note: this should likely be a global service object, not creating a new one each time this command is called

            MakeObject(context, new IReceiver(context));

            return ResultCode.Success;
        }
    }
}