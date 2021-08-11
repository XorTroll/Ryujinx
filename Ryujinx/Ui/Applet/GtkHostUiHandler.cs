using Gtk;
using Ryujinx.HLE;
using Ryujinx.HLE.HOS;
using Ryujinx.HLE.HOS.Services.Am.Applet.ApplicationProxy;
using Ryujinx.Ui.Widgets;
using System;
using System.Threading;

namespace Ryujinx.Ui.Applet
{
    internal class GtkHostUiHandler : IHostUiHandler
    {
        private readonly Window _parent;

        public GtkHostUiHandler(Window parent)
        {
            _parent = parent;
        }

        public bool DisplayMessageDialog(string title, string message)
        {
            ManualResetEvent dialogCloseEvent = new ManualResetEvent(false);

            bool okPressed = false;

            Application.Invoke(delegate
            {
                MessageDialog msgDialog = null;

                try
                {
                    msgDialog = new MessageDialog(_parent, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, null)
                    {
                        Title     = title,
                        Text      = message,
                        UseMarkup = true
                    };

                    msgDialog.SetDefaultSize(400, 0);

                    msgDialog.Response += (object o, ResponseArgs args) =>
                    {
                        if (args.ResponseId == ResponseType.Ok)
                        {
                            okPressed = true;
                        }

                        dialogCloseEvent.Set();
                        msgDialog?.Dispose();
                    };

                    msgDialog.Show();
                }
                catch (Exception ex)
                {
                    GtkDialog.CreateErrorDialog($"Error displaying Message Dialog: {ex}");

                    dialogCloseEvent.Set();
                }
            });

            dialogCloseEvent.WaitOne();

            return okPressed;
        }

        public void ExecuteProgram(ProgramSpecifyKind kind, ulong value)
        {
            Horizon.Instance.Device.Configuration.UserChannelPersistence.ExecuteProgram(kind, value);
            ((MainWindow)_parent).RendererWidget?.Exit();
        }
    }
}