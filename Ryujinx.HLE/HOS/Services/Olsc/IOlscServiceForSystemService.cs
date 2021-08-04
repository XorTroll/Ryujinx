namespace Ryujinx.HLE.HOS.Services.Olsc
{
    [Service("olsc:s")] // 4.0.0+
    class IOlscServiceForSystemService : IpcService
    {
        [CommandHipc(0)]
        // GetTransferTaskListController() -> object<nn::olsc::srv::ITransferTaskListController>
        public ResultCode GetTransferTaskListController(ServiceCtx context)
        {
            MakeObject(context, new ITransferTaskListController());

            return ResultCode.Success;
        }
    }
}