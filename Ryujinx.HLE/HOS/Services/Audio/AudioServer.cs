﻿using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Audio
{
    class AudioServer : ServerManager
    {
        public AudioServer() : base("audio", 0x0100000000000014, 44) { }

        public override Dictionary<string, Func<IpcService>> ServiceTable => new()
        {
            { "audout:u", () => new AudioOutManagerServer() },
            { "audout:a", () => new IAudioOutManagerForApplet() },
            { "audout:d", () => new IAudioOutManagerForDebugger() },
            { "audin:u", () => new AudioInManagerServer() },
            { "audin:a", () => new IAudioInManagerForApplet() },
            { "audin:d", () => new IAudioInManagerForDebugger() },
            { "audrec:u", () => new IFinalOutputRecorderManager() },
            { "audrec:a", () => new IFinalOutputRecorderManagerForApplet() },
            { "audrec:d", () => new IFinalOutputRecorderManagerForDebugger() },
            { "audren:u", () => new AudioRendererManagerServer() },
            { "audren:a", () => new IAudioRendererManagerForApplet() },
            { "audren:d", () => new IAudioRendererManagerForDebugger() },
            { "auddev", () => new IAudioSnoopManager() },
            { "audctl", () => new IAudioController() },
            { "cocecctl", () => new ICodecController() },
            { "hwopus", () => new IHardwareOpusDecoderManager() }
        };
    }
}
