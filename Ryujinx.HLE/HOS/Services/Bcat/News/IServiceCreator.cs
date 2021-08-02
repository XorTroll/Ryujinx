namespace Ryujinx.HLE.HOS.Services.Bcat.News
{
    [Service("news:a")]
    [Service("news:c")]
    [Service("news:m")]
    [Service("news:p")]
    [Service("news:v")]
    class IServiceCreator : IpcService
    {
        public IServiceCreator() { }

        // TODO: permissions depending on service type

        [CommandHipc(0)]
        // CreateNewsService() -> object<nn::news::detail::ipc::INewsService>
        public ResultCode CreateNewsService(ServiceCtx context)
        {
            MakeObject(context, new INewsService());

            return ResultCode.Success;
        }

        [CommandHipc(1)]
        // CreateNewlyArrivedEventHolder() -> object<nn::news::detail::ipc::ICreateNewlyArrivedEventHolder>
        public ResultCode CreateNewlyArrivedEventHolder(ServiceCtx context)
        {
            MakeObject(context, new INewlyArrivedEventHolder(context.Device.System.KernelContext));

            return ResultCode.Success;
        }

        [CommandHipc(2)]
        // CreateNewsDataService() -> object<nn::news::detail::ipc::INewsDataService>
        public ResultCode CreateNewsDataService(ServiceCtx context)
        {
            MakeObject(context, new INewsDataService());

            return ResultCode.Success;
        }

        [CommandHipc(3)]
        // CreateNewsDatabaseService() -> object<nn::news::detail::ipc::INewsDatabaseService>
        public ResultCode CreateNewsDatabaseService(ServiceCtx context)
        {
            MakeObject(context, new INewsDatabaseService());

            return ResultCode.Success;
        }

        [CommandHipc(4)]
        // CreateOverwriteEventHolder() -> object<nn::news::detail::ipc::IOverwriteEventHolder>
        public ResultCode CreateOverwriteEventHolder(ServiceCtx context)
        {
            MakeObject(context, new IOverwriteEventHolder(context.Device.System.KernelContext));

            return ResultCode.Success;
        }
    }
}