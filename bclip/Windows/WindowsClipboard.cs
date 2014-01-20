using System;
using bclip.Model;

namespace bclip.Windows
 
{
    class WindowsClipboard:ClipboardBase
    {
        #region constants
        public static int KEY_V = 0x0056;
        public static int MOD_CONTROL = 0x0002;
        #endregion

        public WindowsClipboard()
        {
            User32Wrapper user32Wrapper = new User32Wrapper();
            user32Wrapper.RegisterHotKey(1, MOD_CONTROL, KEY_V, OnHotKeyPressed);
            user32Wrapper.RegisterClipboard(OnDrawClipboard);
        }

        public static void SendUndo()
        {
            User32Wrapper.SendUndo();
        }

        public void OnHotKeyPressed(object sender, EventArgs e)
        {
            OnPasteDetected();
            User32Wrapper.SendPaste();
        }

        public void OnDrawClipboard(object sender, EventArgs e)
        {
            CopyItem copyItem = CopyItem.FromClipboard();
            OnCopyDetected(copyItem);
        }
    }
}