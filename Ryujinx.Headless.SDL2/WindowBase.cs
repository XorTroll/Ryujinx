﻿using ARMeilleure.Translation;
using Ryujinx.Common.Configuration;
using Ryujinx.Common.Configuration.Hid;
using Ryujinx.Common.Logging;
using Ryujinx.Graphics.GAL;
using Ryujinx.HLE;
using Ryujinx.HLE.HOS;
using Ryujinx.HLE.HOS.Services.Am.Applet.ApplicationProxy;
using Ryujinx.Input;
using Ryujinx.Input.HLE;
using Ryujinx.SDL2.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using static SDL2.SDL;

namespace Ryujinx.Headless.SDL2
{
    abstract class WindowBase : IHostUiHandler, IDisposable
    {
        protected const int DefaultWidth = 1280;
        protected const int DefaultHeight = 720;
        private const SDL_WindowFlags DefaultFlags = SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS | SDL_WindowFlags.SDL_WINDOW_SHOWN;
        private const int TargetFps = 60;

        public NpadManager NpadManager { get; }
        public TouchScreenManager TouchScreenManager { get; }
        public IRenderer Renderer { get; private set; }

        public event EventHandler<StatusUpdatedEventArgs> StatusUpdatedEvent;

        protected IntPtr WindowHandle { get; set; }
        protected SDL2MouseDriver MouseDriver;
        private InputManager _inputManager;
        private IKeyboard _keyboardInterface;
        private GraphicsDebugLevel _glLogLevel;
        private readonly Stopwatch _chrono;
        private readonly long _ticksPerFrame;
        private readonly ManualResetEvent _exitEvent;

        private long _ticks;
        private bool _isActive;
        private bool _isStopped;
        private uint _windowId;

        private string _gpuVendorName;

        private AspectRatio _aspectRatio;
        private bool _enableMouse;

        public WindowBase(InputManager inputManager, GraphicsDebugLevel glLogLevel, AspectRatio aspectRatio, bool enableMouse)
        {
            MouseDriver = new SDL2MouseDriver();
            _inputManager = inputManager;
            _inputManager.SetMouseDriver(MouseDriver);
            NpadManager = _inputManager.CreateNpadManager();
            TouchScreenManager = _inputManager.CreateTouchScreenManager();
            _keyboardInterface = (IKeyboard)_inputManager.KeyboardDriver.GetGamepad("0");
            _glLogLevel = glLogLevel;
            _chrono = new Stopwatch();
            _ticksPerFrame = Stopwatch.Frequency / TargetFps;
            _exitEvent = new ManualResetEvent(false);
            _aspectRatio = aspectRatio;
            _enableMouse = enableMouse;

            SDL2Driver.Instance.Initialize();
        }

        public void Initialize(List<InputConfig> inputConfigs, bool enableKeyboard, bool enableMouse)
        {
            Renderer = Horizon.Instance.Device.Gpu.Renderer;

            NpadManager.Initialize(inputConfigs, enableKeyboard, enableMouse);
            TouchScreenManager.Initialize(Horizon.Instance.Device);
        }

        private void InitializeWindow()
        {
            string titleNameSection = string.IsNullOrWhiteSpace(Horizon.Instance.Device.Application.TitleName) ? string.Empty
                : $" - {Horizon.Instance.Device.Application.TitleName}";

            string titleVersionSection = string.IsNullOrWhiteSpace(Horizon.Instance.Device.Application.DisplayVersion) ? string.Empty
                : $" v{Horizon.Instance.Device.Application.DisplayVersion}";

            string titleIdSection = string.IsNullOrWhiteSpace(Horizon.Instance.Device.Application.TitleIdText) ? string.Empty
                : $" ({Horizon.Instance.Device.Application.TitleIdText.ToUpper()})";

            string titleArchSection = Horizon.Instance.Device.Application.TitleIs64Bit ? " (64-bit)" : " (32-bit)";

            WindowHandle = SDL_CreateWindow($"Ryujinx {Program.Version}{titleNameSection}{titleVersionSection}{titleIdSection}{titleArchSection}", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, DefaultWidth, DefaultHeight, DefaultFlags | GetWindowFlags());

            if (WindowHandle == IntPtr.Zero)
            {
                string errorMessage = $"SDL_CreateWindow failed with error \"{SDL_GetError()}\"";

                Logger.Error?.Print(LogClass.Application, errorMessage);

                throw new Exception(errorMessage);
            }

            _windowId = SDL_GetWindowID(WindowHandle);
            SDL2Driver.Instance.RegisterWindow(_windowId, HandleWindowEvent);
        }

        private void HandleWindowEvent(SDL_Event evnt)
        {
            if (evnt.type == SDL_EventType.SDL_WINDOWEVENT)
            {
                switch (evnt.window.windowEvent)
                {
                    case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                        Renderer?.Window.SetSize(evnt.window.data1, evnt.window.data2);
                        MouseDriver.SetClientSize(evnt.window.data1, evnt.window.data2);
                        break;
                    case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                        Exit();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                MouseDriver.Update(evnt);
            }
        }

        protected abstract void InitializeRenderer();

        protected abstract void FinalizeRenderer();

        protected abstract void SwapBuffers();

        protected abstract string GetGpuVendorName();

        public abstract SDL_WindowFlags GetWindowFlags();

        public void Render()
        {
            InitializeRenderer();

            Horizon.Instance.Device.Gpu.Renderer.Initialize(_glLogLevel);

            _gpuVendorName = GetGpuVendorName();

            Horizon.Instance.Device.Gpu.InitializeShaderCache();
            Translator.IsReadyForTranslation.Set();

            while (_isActive)
            {
                if (_isStopped)
                {
                    return;
                }

                _ticks += _chrono.ElapsedTicks;

                _chrono.Restart();

                if (Horizon.Instance.Device.WaitFifo())
                {
                    Horizon.Instance.Device.Statistics.RecordFifoStart();
                    Horizon.Instance.Device.ProcessFrame();
                    Horizon.Instance.Device.Statistics.RecordFifoEnd();
                }

                while (Horizon.Instance.Device.ConsumeFrameAvailable())
                {
                    Horizon.Instance.Device.PresentFrame(SwapBuffers);
                }

                if (_ticks >= _ticksPerFrame)
                {
                    string dockedMode = Horizon.Instance.Device.System.State.DockedMode ? "Docked" : "Handheld";
                    float scale = Graphics.Gpu.GraphicsConfig.ResScale;
                    if (scale != 1)
                    {
                        dockedMode += $" ({scale}x)";
                    }

                    StatusUpdatedEvent?.Invoke(this, new StatusUpdatedEventArgs(
                        Horizon.Instance.Device.EnableDeviceVsync,
                        dockedMode,
                        Horizon.Instance.Device.Configuration.AspectRatio.ToText(),
                        $"Game: {Horizon.Instance.Device.Statistics.GetGameFrameRate():00.00} FPS",
                        $"FIFO: {Horizon.Instance.Device.Statistics.GetFifoPercent():0.00} %",
                        $"GPU: {_gpuVendorName}"));

                    _ticks = Math.Min(_ticks - _ticksPerFrame, _ticksPerFrame);
                }
            }

            FinalizeRenderer();
        }

        public void Exit()
        {
            TouchScreenManager?.Dispose();
            NpadManager?.Dispose();

            if (_isStopped)
            {
                return;
            }

            _isStopped = true;
            _isActive = false;

            _exitEvent.WaitOne();
            _exitEvent.Dispose();
        }

        public void MainLoop()
        {
            while (_isActive)
            {
                UpdateFrame();

                SDL_PumpEvents();

                // Polling becomes expensive if it's not slept
                Thread.Sleep(1);
            }

            _exitEvent.Set();
        }

        private void NVStutterWorkaround()
        {
            while (_isActive)
            {
                // When NVIDIA Threaded Optimization is on, the driver will snapshot all threads in the system whenever the application creates any new ones.
                // The ThreadPool has something called a "GateThread" which terminates itself after some inactivity.
                // However, it immediately starts up again, since the rules regarding when to terminate and when to start differ.
                // This creates a new thread every second or so.
                // The main problem with this is that the thread snapshot can take 70ms, is on the OpenGL thread and will delay rendering any graphics.
                // This is a little over budget on a frame time of 16ms, so creates a large stutter.
                // The solution is to keep the ThreadPool active so that it never has a reason to terminate the GateThread.

                // TODO: This should be removed when the issue with the GateThread is resolved.

                ThreadPool.QueueUserWorkItem((state) => { });
                Thread.Sleep(300);
            }
        }

        private bool UpdateFrame()
        {
            if (!_isActive)
            {
                return true;
            }

            if (_isStopped)
            {
                return false;
            }

            NpadManager.Update();

            // Touchscreen
            bool hasTouch = false;

            // Get screen touch position
            if (!_enableMouse)
            {
                hasTouch = TouchScreenManager.Update(true, (_inputManager.MouseDriver as SDL2MouseDriver).IsButtonPressed(MouseButton.Button1), _aspectRatio.ToFloat());
            }

            if (!hasTouch)
            {
                TouchScreenManager.Update(false);
            }

            Horizon.Instance.Device.Hid.DebugPad.Update();

            return true;
        }

        public void Execute()
        {
            _chrono.Restart();
            _isActive = true;

            InitializeWindow();

            Thread renderLoopThread = new Thread(Render)
            {
                Name = "GUI.RenderLoop"
            };
            renderLoopThread.Start();

            Thread nvStutterWorkaround = new Thread(NVStutterWorkaround)
            {
                Name = "GUI.NVStutterWorkaround"
            };
            nvStutterWorkaround.Start();

            MainLoop();

            renderLoopThread.Join();
            nvStutterWorkaround.Join();

            Exit();
        }

        public bool DisplayMessageDialog(string title, string message)
        {
            SDL_ShowSimpleMessageBox(SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION, title, message, WindowHandle);

            return true;
        }

        public void ExecuteProgram(ProgramSpecifyKind kind, ulong value)
        {
            Horizon.Instance.Device.Configuration.UserChannelPersistence.ExecuteProgram(kind, value);

            Exit();
        }

        public bool DisplayErrorAppletDialog(string title, string message, string[] buttonsText)
        {
            SDL_MessageBoxData data = new SDL_MessageBoxData
            {
                title = title,
                message = message,
                buttons = new SDL_MessageBoxButtonData[buttonsText.Length],
                numbuttons = buttonsText.Length,
                window = WindowHandle
            };

            for (int i = 0; i < buttonsText.Length; i++)
            {
                data.buttons[i] = new SDL_MessageBoxButtonData
                {
                    buttonid = i,
                    text = buttonsText[i]
                };
            }

            SDL_ShowMessageBox(ref data, out int _);

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isActive = false;
                TouchScreenManager?.Dispose();
                NpadManager.Dispose();

                SDL2Driver.Instance.UnregisterWindow(_windowId);

                SDL_DestroyWindow(WindowHandle);

                SDL2Driver.Instance.Dispose();
            }
        }
    }
}
