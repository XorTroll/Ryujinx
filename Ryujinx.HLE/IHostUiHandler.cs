using Ryujinx.HLE.HOS.Services.Am.Applet.ApplicationProxy;

namespace Ryujinx.HLE
{
    public interface IHostUiHandler
    {
        /// <summary>
        /// Displays a Message Dialog box to the user and blocks until it is closed.
        /// </summary>
        /// <returns>True when OK is pressed, False otherwise.</returns>
        bool DisplayMessageDialog(string title, string message);

        /// <summary>
        /// Tell the UI that we need to transisition to another program.
        /// </summary>
        /// <param name="kind">The program kind.</param>
        /// <param name="value">The value associated to the <paramref name="kind"/>.</param>
        void ExecuteProgram(ProgramSpecifyKind kind, ulong value);

        /// Displays a Message Dialog box specific to Error Applet and blocks until it is closed.
        /// </summary>
        /// <returns>False when OK is pressed, True when another button (Details) is pressed.</returns>
        bool DisplayErrorAppletDialog(string title, string message, string[] buttonsText);
    }
}