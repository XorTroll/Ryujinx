﻿using Ryujinx.Common.Logging;
using Ryujinx.HLE.FileSystem.Content;
using Ryujinx.HLE.HOS.Services.Settings;
using Ryujinx.Ui.Widgets;
using System;
using System.IO;

namespace Ryujinx.Ui.Helper
{
    /// <summary>
    /// Ensure installation validity
    /// </summary>
    static class SetupValidator
    {
        public static bool IsFirmwareValid(ContentManager contentManager, out UserError error)
        {
            bool hasFirmware = contentManager.TryGetCurrentFirmwareVersion(out _);

            if (hasFirmware)
            {
                error = UserError.Success;

                return true;
            }
            else
            {
                error = UserError.NoFirmware;

                return false;
            }
        }

        public static bool CanFixStartApplication(ContentManager contentManager, string baseApplicationPath, UserError error, out FirmwareVersion firmwareVersion)
        {
            firmwareVersion = new FirmwareVersion();
            bool ok;

            try
            {
                ok = contentManager.VerifyFirmwarePackage(baseApplicationPath, out firmwareVersion);
            }
            catch (Exception)
            {
                ok = false;
            }

            return error == UserError.NoFirmware && Path.GetExtension(baseApplicationPath).ToLowerInvariant() == ".xci" && ok;
        }

        public static bool TryFixStartApplication(ContentManager contentManager, string baseApplicationPath, UserError error, out UserError outError)
        {
            if (error == UserError.NoFirmware)
            {
                string baseApplicationExtension = Path.GetExtension(baseApplicationPath).ToLowerInvariant();

                // If the target app to start is a XCI, try to install firmware from it
                if (baseApplicationExtension == ".xci")
                {
                    FirmwareVersion firmwareVersion;
                    bool ok = false;

                    try
                    {
                        ok = contentManager.VerifyFirmwarePackage(baseApplicationPath, out firmwareVersion);
                    }
                    catch (Exception)
                    {
                        firmwareVersion = new FirmwareVersion();
                    }

                    // The XCI is a valid firmware package, try to install the firmware from it!
                    if (ok)
                    {
                        try
                        {
                            var displayVersion = firmwareVersion.GetDisplayVersion();

                            Logger.Info?.Print(LogClass.Application, $"Installing firmware {displayVersion}");

                            contentManager.InstallFirmware(baseApplicationPath);

                            Logger.Info?.Print(LogClass.Application, $"System version {displayVersion} successfully installed.");

                            outError = UserError.Success;

                            return true;
                        }
                        catch (Exception) { }
                    }

                    outError = error;

                    return false;
                }
            }

            outError = error;

            return false;
        }

        public static bool CanStartApplication(ContentManager contentManager, string baseApplicationPath, out UserError error)
        {
            if (Directory.Exists(baseApplicationPath) || File.Exists(baseApplicationPath))
            {
                string baseApplicationExtension = Path.GetExtension(baseApplicationPath).ToLowerInvariant();

                // NOTE: We don't force homebrew developers to install a system firmware.
                if (baseApplicationExtension == ".nro" || baseApplicationExtension == ".nso")
                {
                    error = UserError.Success;

                    return true;
                }

                return IsFirmwareValid(contentManager, out error);
            }
            else
            {
                error = UserError.ApplicationNotFound;

                return false;
            }
        }
    }
}
