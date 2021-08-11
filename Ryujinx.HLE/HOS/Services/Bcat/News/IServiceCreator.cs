namespace Ryujinx.HLE.HOS.Services.Bcat.News
{
    [Service("news:a")]
    [Service("news:c")]
    [Service("news:m")]
    [Service("news:p")]
    [Service("news:v")]
    class IServiceCreator : ServiceCreatorBase
    {
        // TODO: permissions depending on service type

        [CommandHipc(0)]
        // CreateNewsService() -> object<nn::news::detail::ipc::INewsService>
        public ResultCode CreateNewsService(ServiceCtx context)
        {
            return CreateNewsServiceImpl(context);
        }

        [CommandHipc(1)]
        // CreateNewlyArrivedEventHolder() -> object<nn::news::detail::ipc::ICreateNewlyArrivedEventHolder>
        public ResultCode CreateNewlyArrivedEventHolder(ServiceCtx context)
        {
            return CreateNewlyArrivedEventHolderImpl(context);
        }

        [CommandHipc(2)]
        // CreateNewsDataService() -> object<nn::news::detail::ipc::INewsDataService>
        public ResultCode CreateNewsDataService(ServiceCtx context)
        {
            return CreateNewsDataServiceImpl(context);
        }

        [CommandHipc(3)]
        // CreateNewsDatabaseService() -> object<nn::news::detail::ipc::INewsDatabaseService>
        public ResultCode CreateNewsDatabaseService(ServiceCtx context)
        {
            return CreateNewsDatabaseServiceImpl(context);
        }

        [CommandHipc(4)]
        // CreateOverwriteEventHolder() -> object<nn::news::detail::ipc::IOverwriteEventHolder>
        public ResultCode CreateOverwriteEventHolder(ServiceCtx context)
        {
            return CreateOverwriteEventHolderImpl(context);
        }
    }
}