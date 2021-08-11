using MsgPack;
using MsgPack.Serialization;
using Ryujinx.Common.Logging;
using Ryujinx.Common.Utilities;
using System.Text;

namespace Ryujinx.HLE.HOS.Services.Bcat.News
{
    class INewsService : ServiceCreatorBase
    {
        public INewsService() { }

        [CommandHipc(10100)]
        // PostLocalNews(buffer)
        public ResultCode PostLocalNews(ServiceCtx context)
        {
            // TODO
            var msgpackBuf = context.Request.SendBuff[0];
            var msgpackData = new byte[msgpackBuf.Size];
            context.Memory.Read(msgpackBuf.Position, msgpackData);

            // NOTE: not printing the msgpack since it prints all the image data byte by byte... :P
            /*
            var builder = new StringBuilder();
            MessagePackObject deserializedReport = MessagePackSerializer.UnpackMessagePackObject(msgpackData);

            builder.AppendLine();
            builder.AppendLine("News log:");

            builder.AppendLine($" Local news: {MessagePackObjectFormatter.Format(deserializedReport)}");

            Logger.Stub?.Print(LogClass.Service, builder.ToString());
            */

            return ResultCode.Success;
        }

        [CommandHipc(30100)]
        // GetSubscriptionStatus() -> u32
        public ResultCode GetSubscriptionStatus(ServiceCtx context)
        {
            context.ResponseData.Write((uint)0);

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }

        [CommandHipc(30200)]
        // IsSystemUpdateRequired() -> bool?
        public ResultCode IsSystemUpdateRequired(ServiceCtx context)
        {
            // TODO

            context.ResponseData.Write(false);

            Logger.Stub?.PrintStub(LogClass.Service);

            return ResultCode.Success;
        }

        [CommandHipc(30900)] // 1.0.0
        // CreateNewlyArrivedEventHolder() -> object<nn::news::detail::ipc::ICreateNewlyArrivedEventHolder>
        public ResultCode CreateNewlyArrivedEventHolder(ServiceCtx context)
        {
            return CreateNewlyArrivedEventHolderImpl(context);
        }

        [CommandHipc(30901)] // 1.0.0
        // CreateNewsDataService() -> object<nn::news::detail::ipc::INewsDataService>
        public ResultCode CreateNewsDataService(ServiceCtx context)
        {
            return CreateNewsDataServiceImpl(context);
        }

        [CommandHipc(30902)] // 1.0.0
        // CreateNewsDatabaseService() -> object<nn::news::detail::ipc::INewsDatabaseService>
        public ResultCode CreateNewsDatabaseService(ServiceCtx context)
        {
            return CreateNewsDatabaseServiceImpl(context);
        }

        [CommandHipc(40101)]
        // RequestAutoSubscription(u64)
        public ResultCode RequestAutoSubscription(ServiceCtx context)
        {
            // TODO

            var unk = context.RequestData.ReadUInt64();

            Logger.Stub?.PrintStub(LogClass.Service, new { unk });

            return ResultCode.Success;
        }
    }
}