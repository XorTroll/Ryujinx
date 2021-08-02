namespace Ryujinx.HLE.HOS.Services.Ns
{
    [Service("ns:am2")]
    [Service("ns:ec")]
    [Service("ns:rid")]
    [Service("ns:rt")]
    [Service("ns:web")]
    class IServiceGetterInterface : IpcService
    {
        public IServiceGetterInterface() { }

        [CommandHipc(7989)]
        // GetReadOnlyApplicationControlDataInterface() -> object<nn::ns::detail::IReadOnlyApplicationControlDataInterface>
        public ResultCode GetReadOnlyApplicationControlDataInterface(ServiceCtx context)
        {
            MakeObject(context, new IReadOnlyApplicationControlDataInterface(context));

            return ResultCode.Success;
        }

        [CommandHipc(7996)]
        // GetApplicationManagerInterface() -> object<nn::ns::detail::IApplicationManagerInterface>
        public ResultCode GetApplicationManagerInterface(ServiceCtx context)
        {
            MakeObject(context, new IApplicationManagerInterface(context.Device.System.KernelContext));

            return ResultCode.Success;
        }

        [CommandHipc(7997)]
        // GetDownloadTaskInterface() -> object<nn::ns::detail::IDownloadTaskInterface>
        public ResultCode GetDownloadTaskInterface(ServiceCtx context)
        {
            MakeObject(context, new IDownloadTaskInterface(context));

            return ResultCode.Success;
        }

        [CommandHipc(7998)]
        // GetContentManagementInterface() -> object<nn::ns::detail::IContentManagementInterface>
        public ResultCode GetContentManagementInterface(ServiceCtx context)
        {
            MakeObject(context, new IContentManagementInterface(context));

            return ResultCode.Success;
        }
    }
}