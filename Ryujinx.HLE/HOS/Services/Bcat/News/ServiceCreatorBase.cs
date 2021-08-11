namespace Ryujinx.HLE.HOS.Services.Bcat.News
{
    class ServiceCreatorBase : IpcService
    {
        // CreateNewsService() -> object<nn::news::detail::ipc::INewsService>
        public ResultCode CreateNewsServiceImpl(ServiceCtx context)
        {
            MakeObject(context, new INewsService());

            return ResultCode.Success;
        }

        // CreateNewlyArrivedEventHolder() -> object<nn::news::detail::ipc::ICreateNewlyArrivedEventHolder>
        public ResultCode CreateNewlyArrivedEventHolderImpl(ServiceCtx context)
        {
            MakeObject(context, new INewlyArrivedEventHolder());

            return ResultCode.Success;
        }

        // CreateNewsDataService() -> object<nn::news::detail::ipc::INewsDataService>
        public ResultCode CreateNewsDataServiceImpl(ServiceCtx context)
        {
            MakeObject(context, new INewsDataService());

            return ResultCode.Success;
        }

        // CreateNewsDatabaseService() -> object<nn::news::detail::ipc::INewsDatabaseService>
        public ResultCode CreateNewsDatabaseServiceImpl(ServiceCtx context)
        {
            MakeObject(context, new INewsDatabaseService());

            return ResultCode.Success;
        }

        // CreateOverwriteEventHolder() -> object<nn::news::detail::ipc::IOverwriteEventHolder>
        public ResultCode CreateOverwriteEventHolderImpl(ServiceCtx context)
        {
            MakeObject(context, new IOverwriteEventHolder());

            return ResultCode.Success;
        }
    }
}