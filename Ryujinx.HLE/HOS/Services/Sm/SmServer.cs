using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Ipc;
using Ryujinx.HLE.HOS.Kernel.Process;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Ryujinx.HLE.HOS.Services.Sm
{
    class SmServer : ServerManager
    {
        public SmServer(Horizon system) : base(system, "sm", 0x0100000000000004, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new();

        protected override void OnStart()
        {
            RegisterNamedPort("sm:", 50, () => new IUserInterface());
            // TODO: sm:m?
        }

        public ResultCode DoGetService(string name, out int serviceHandle)
        {
            serviceHandle = 0;

            if (string.IsNullOrEmpty(name))
            {
                return ResultCode.InvalidName;
            }

            foreach (var server in _system.GetServerList())
            {
                if (server.ServiceTable.ContainsKey(name))
                {
                    var serviceSession = new KSession(_system.KernelContext);
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

            // TODO: static config
            /*
            if (context.Device.Configuration.IgnoreMissingServices)
            {
                Logger.Warning?.Print(LogClass.Service, $"Missing service {name} ignored");
            }
            */
            throw new NotImplementedException(name);
        }
    }
}
