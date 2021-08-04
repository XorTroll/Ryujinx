using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Ipc;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Sm
{
    class SmServer : ServerManager
    {
        public SmServer() : base("sm", 0x0100000000000004, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "sm:m", () => new IManagerInterface() }
        };

        protected override void OnStart()
        {
            RegisterNamedPort("sm:", 50, () => new IUserInterface());
        }

        public ResultCode DoGetService(string name, out int serviceHandle)
        {
            serviceHandle = 0;

            if (string.IsNullOrEmpty(name))
            {
                return ResultCode.InvalidName;
            }

            foreach (var server in Horizon.Instance.GetServerList())
            {
                if (server.ServiceTable.ContainsKey(name))
                {
                    var serviceSession = new KSession(Horizon.Instance.KernelContext);
                    var serviceObj = server.ServiceTable[name].Invoke();

                    server.AddSessionObject(serviceSession.ServerSession, serviceObj);

                    if (_selfProcess.HandleTable.GenerateHandle(serviceSession.ClientSession, out serviceHandle) != KernelResult.Success)
                    {
                        throw new InvalidOperationException("Out of handles!");
                    }

                    serviceSession.ServerSession.DecrementReferenceCount();
                    serviceSession.ClientSession.DecrementReferenceCount();

                    return ResultCode.Success;
                }
            }

            throw new NotImplementedException(name);
        }
    }
}
