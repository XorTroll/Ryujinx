using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.HLE.HOS.Kernel.Ipc;
using Ryujinx.HLE.HOS.Kernel.Process;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Ryujinx.HLE.HOS.Services
{
    abstract class ServerManager : IDisposable
    {
        // Must be the maximum value used by services (highest one know is the one used by nvservices = 0x8000).
        // Having a size that is too low will cause failures as data copy will fail if the receiving buffer is
        // not large enough.
        private const int PointerBufferSize = 0x8000;

        private const int HeapSize = 0x200000;

        private const ProcessCreationFlags ProcessFlags =
            ProcessCreationFlags.EnableAslr |
            ProcessCreationFlags.AddressSpace64Bit |
            ProcessCreationFlags.Is64Bit |
            ProcessCreationFlags.PoolPartitionSystem;

        private readonly static int[] DefaultCapabilities = new int[]
        {
            0x030363F7,
            0x1FFFFFCF,
            0x207FFFEF,
            0x47E0060F,
            0x0048BFFF,
            0x01007FFF
        };

        protected readonly Horizon _system;
        protected KProcess _selfProcess;
        protected KThread _selfMainThread;
        protected object _serverLoopLock = new object();

        private ulong _heapAddress = 0;
        private int _extraThreadCount = 0;
        private readonly List<int> _sessionHandles = new List<int>();
        private readonly List<int> _portHandles = new List<int>();
        private readonly Dictionary<int, IpcService> _sessions = new Dictionary<int, IpcService>();
        private readonly Dictionary<int, Func<IpcService>> _ports = new Dictionary<int, Func<IpcService>>();

        public ManualResetEvent InitDone { get; }
        public string Name => _selfProcess?.Name;

        public ServerManager(Horizon system, string processName, ulong programId, int mainThreadPriority, int extraThreadCount = 0)
        {
            InitDone = new ManualResetEvent(false);
            _system = system;
            _extraThreadCount = extraThreadCount;

            var creationInfo = new ProcessCreationInfo(processName, 1, programId, 0x8000000, 1, ProcessFlags, 0, 0);
            KernelStatic.StartInitialProcess(system.KernelContext, creationInfo, DefaultCapabilities, mainThreadPriority, MainThread);
        }

        public abstract Dictionary<string, Func<IpcService>> ServiceTable { get; }

        protected virtual void OnStart() { }

        public void AddSessionObject(KServerSession serverSession, IpcService obj)
        {
            // Ensure that the sever loop is running.
            InitDone.WaitOne();

            _selfProcess.HandleTable.GenerateHandle(serverSession, out int serverSessionHandle);
            AddSessionObject(serverSessionHandle, obj);
        }

        public void AddSessionObject(int serverSessionHandle, IpcService obj)
        {
            obj.TrySetServer(this);
            _sessionHandles.Add(serverSessionHandle);
            _sessions.Add(serverSessionHandle, obj);
        }

        private void RegisterPort(int portHandle, Func<IpcService> objFactory)
        {
            _portHandles.Add(portHandle);
            _ports.Add(portHandle, objFactory);
        }

        public void RegisterNamedPort(string portName, int maxSessions, Func<IpcService> objFactory)
        {
            _system.KernelContext.Syscall.ManageNamedPort(portName, maxSessions, out var portHandle);
            RegisterPort(portHandle, objFactory);
        }

        private void MainThread()
        {
            _selfProcess = KernelStatic.GetCurrentProcess();
            _selfMainThread = KernelStatic.GetCurrentThread();
            _system.KernelContext.Syscall.SetHeapSize(HeapSize, out _heapAddress);
            OnStart();
            InitDone.Set();

            for(var i = 0; i < _extraThreadCount; i++)
            {
                // TODO: spawn multiple threads running ServerLoop
            }

            ServerLoop();
        }

        private void ServerLoop()
        {
            var thread = KernelStatic.GetCurrentThread();
            var messagePtr = thread.TlsAddress;

            _selfProcess.CpuMemory.Write(messagePtr + 0x0, 0);
            _selfProcess.CpuMemory.Write(messagePtr + 0x4, 2 << 10);
            _selfProcess.CpuMemory.Write(messagePtr + 0x8, _heapAddress | ((ulong)PointerBufferSize << 48));

            int replyTargetHandle = 0;

            while (true)
            {
                lock(_serverLoopLock)
                {
                    int[] portHandles = _portHandles.ToArray();
                    int[] sessionHandles = _sessionHandles.ToArray();
                    int[] handles = new int[portHandles.Length + sessionHandles.Length];

                    portHandles.CopyTo(handles, 0);
                    sessionHandles.CopyTo(handles, portHandles.Length);

                    // We still need a timeout here to allow the service to pick up and listen new sessions...
                    var rc = _system.KernelContext.Syscall.ReplyAndReceive(handles, replyTargetHandle, 1000000L, out int signaledIndex);
                    thread.HandlePostSyscall();

                    if (!thread.Context.Running)
                    {
                        break;
                    }

                    replyTargetHandle = 0;

                    if(rc == KernelResult.Success)
                    {
                        if (signaledIndex >= portHandles.Length)
                        {
                            // We got a IPC request, process it, pass to the appropriate service if needed.
                            int signaledHandle = handles[signaledIndex];

                            if (Process(signaledHandle, _heapAddress))
                            {
                                replyTargetHandle = signaledHandle;
                            }
                        }
                        else
                        {
                            // We got a new connection, accept the session to allow servicing future requests.
                            rc = _system.KernelContext.Syscall.AcceptSession(handles[signaledIndex], out int serverSessionHandle);
                            if (rc == KernelResult.Success)
                            {
                                var obj = _ports[handles[signaledIndex]].Invoke();
                                obj.TrySetServer(this);
                                AddSessionObject(serverSessionHandle, obj);
                            }

                            _selfProcess.CpuMemory.Write(messagePtr + 0x0, 0);
                            _selfProcess.CpuMemory.Write(messagePtr + 0x4, 2 << 10);
                            _selfProcess.CpuMemory.Write(messagePtr + 0x8, _heapAddress | ((ulong)PointerBufferSize << 48));
                        }
                    }
                }
            }

            Dispose();
        }

        private bool Process(int serverSessionHandle, ulong recvListAddr)
        {
            KProcess process = KernelStatic.GetCurrentProcess();
            KThread thread = KernelStatic.GetCurrentThread();
            ulong messagePtr = thread.TlsAddress;
            ulong messageSize = 0x100;

            byte[] reqData = new byte[messageSize];

            process.CpuMemory.Read(messagePtr, reqData);

            IpcMessage request = new IpcMessage(reqData, (long)messagePtr);
            IpcMessage response = new IpcMessage();

            ulong tempAddr = recvListAddr;
            int sizesOffset = request.RawData.Length - ((request.RecvListBuff.Count * 2 + 3) & ~3);

            bool noReceive = true;

            for (int i = 0; i < request.ReceiveBuff.Count; i++)
            {
                noReceive &= (request.ReceiveBuff[i].Position == 0);
            }

            if (noReceive)
            {
                for (int i = 0; i < request.RecvListBuff.Count; i++)
                {
                    ulong size = (ulong)BinaryPrimitives.ReadInt16LittleEndian(request.RawData.AsSpan().Slice(sizesOffset + i * 2, 2));

                    response.PtrBuff.Add(new IpcPtrBuffDesc(tempAddr, (uint)i, size));

                    request.RecvListBuff[i] = new IpcRecvListBuffDesc(tempAddr, size);

                    tempAddr += size;
                }
            }
            5
            var shouldReply = true;
            var isTipcProtocol = false;

            using (MemoryStream raw = new MemoryStream(request.RawData))
            {
                BinaryReader reqReader = new BinaryReader(raw);

                if (request.Type == IpcMessageType.HipcRequest ||
                    request.Type == IpcMessageType.HipcRequestWithContext)
                {
                    response.Type = IpcMessageType.HipcResponse;

                    using (MemoryStream resMs = new MemoryStream())
                    {
                        BinaryWriter resWriter = new BinaryWriter(resMs);

                        ServiceCtx context = new ServiceCtx(
                            _system.KernelContext.Device,
                            process,
                            process.CpuMemory,
                            thread,
                            request,
                            response,
                            reqReader,
                            resWriter,
                            isTipcProtocol);

                        _sessions[serverSessionHandle].CallHipcMethod(context);

                        response.RawData = resMs.ToArray();
                    }
                }
                else if (request.Type == IpcMessageType.HipcControl ||
                         request.Type == IpcMessageType.HipcControlWithContext)
                {
                    uint magic = (uint)reqReader.ReadUInt64();
                    uint cmdId = (uint)reqReader.ReadUInt64();

                    switch (cmdId)
                    {
                        case 0:
                            request = FillResponse(response, 0, _sessions[serverSessionHandle].ConvertToDomain());
                            break;

                        case 3:
                            request = FillResponse(response, 0, PointerBufferSize);
                            break;

                        // TODO: Whats the difference between IpcDuplicateSession/Ex?
                        case 2:
                        case 4:
                            int unknown = reqReader.ReadInt32();

                            _system.KernelContext.Syscall.CreateSession(false, 0, out int dupServerSessionHandle, out int dupClientSessionHandle);

                            AddSessionObject(dupServerSessionHandle, _sessions[serverSessionHandle]);

                            response.HandleDesc = IpcHandleDesc.MakeMove(dupClientSessionHandle);

                            request = FillResponse(response, 0);

                            break;

                        default: throw new NotImplementedException(cmdId.ToString());
                    }
                }
                else if (request.Type == IpcMessageType.HipcCloseSession || request.Type == IpcMessageType.TipcCloseSession)
                {
                    _system.KernelContext.Syscall.CloseHandle(serverSessionHandle);
                    _sessionHandles.Remove(serverSessionHandle);
                    IpcService service = _sessions[serverSessionHandle];
                    if (service is IDisposable disposableObj)
                    {
                        disposableObj.Dispose();
                    }
                    _sessions.Remove(serverSessionHandle);
                    shouldReply = false;
                }
                // If the type is past 0xF, we are using TIPC
                else if (request.Type > IpcMessageType.TipcCloseSession)
                {
                    isTipcProtocol = true;

                    // Response type is always the same as request on TIPC.
                    response.Type = request.Type;

                    using (MemoryStream resMs = new MemoryStream())
                    {
                        BinaryWriter resWriter = new BinaryWriter(resMs);

                        ServiceCtx context = new ServiceCtx(
                            _system.KernelContext.Device,
                            process,
                            process.CpuMemory,
                            thread,
                            request,
                            response,
                            reqReader,
                            resWriter,
                            isTipcProtocol);

                        _sessions[serverSessionHandle].CallTipcMethod(context);

                        response.RawData = resMs.ToArray();
                    }

                    process.CpuMemory.Write(messagePtr, response.GetBytesTipc());
                }
                else
                {
                    throw new NotImplementedException(request.Type.ToString());
                }

                if (!isTipcProtocol)
                {
                    process.CpuMemory.Write(messagePtr, response.GetBytes((long)messagePtr, recvListAddr | ((ulong)PointerBufferSize << 48)));
                }

                return shouldReply;
            }
        }

        private static IpcMessage FillResponse(IpcMessage response, long result, params int[] values)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(ms);

                foreach (int value in values)
                {
                    writer.Write(value);
                }

                return FillResponse(response, result, ms.ToArray());
            }
        }

        private static IpcMessage FillResponse(IpcMessage response, long result, byte[] data = null)
        {
            response.Type = IpcMessageType.HipcResponse;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(ms);

                writer.Write(IpcMagic.Sfco);
                writer.Write(result);

                if (data != null)
                {
                    writer.Write(data);
                }

                response.RawData = ms.ToArray();
            }

            return response;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (IpcService service in _sessions.Values)
                {
                    if (service is IDisposable disposableObj)
                    {
                        disposableObj.Dispose();
                    }

                    service.DestroyAtExit();
                }

                _sessions.Clear();

                InitDone.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
