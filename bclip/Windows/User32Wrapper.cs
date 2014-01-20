using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace bclip.Windows
{
    internal sealed class User32Wrapper : Form
    {
        #region Constants 

        /*defined in winuser.h*/

        //clipboard related
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x030D;

        //hotkey related
        private const int WM_HOTKEY = 0x312;

        #endregion Constants

        #region External Functions

        [DllImport("User32.dll")]
        private static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll")]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);


        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", EntryPoint = "GetGUIThreadInfo")]
        private static extern bool GetGUIThreadInfo(int tId, out GuiThreadInfo threadInfo);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, StringBuilder lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, string lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        #endregion External Functions

        #region EventHandlers

        private EventHandler _onDrawClipboard;
        private EventHandler _onHotKeyPressed;

        #endregion EventHandlers

        private IntPtr _nextClipboardViewer;

        public void RegisterClipboard(EventHandler onDrawClipboard)
        {
            _onDrawClipboard = onDrawClipboard;
            _nextClipboardViewer = (IntPtr) SetClipboardViewer((int) Handle);
        }

        private static GuiThreadInfo GetThreadInfo(int tid)
        {
            var tinfo = new GuiThreadInfo();
            tinfo.cbSize = Marshal.SizeOf(tinfo);
            GetGUIThreadInfo(tid, out tinfo);
            return tinfo;
        }

        /*
        public static string FocusedClassName()
        {
            GuiThreadInfo info = GetThreadInfo((int) 0);
            StringBuilder className = new StringBuilder(100);
            var x = GetClassName(info.hwndCapred, className, 100);
            return className.ToString();

        }*/

        public static IntPtr FocusedHandle()
        {
            GuiThreadInfo info = GetThreadInfo(0);
            return info.hwndFocus;
        }

        public new void Dispose()
        {
            UnregisterHotKey(Handle, 1);
            ChangeClipboardChain(Handle, _nextClipboardViewer);
            base.Dispose();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    _onHotKeyPressed(this, new EventArgs());
                    break;

                case WM_DRAWCLIPBOARD:
                    _onDrawClipboard(this, new EventArgs());
                    SendMessage(_nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == _nextClipboardViewer)
                        _nextClipboardViewer = m.LParam;
                    else
                        SendMessage(_nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;


                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        public void RegisterHotKey(int id, int modifiers, int keys, EventHandler onHotKeyPressed)
        {
            if (_onHotKeyPressed != null)
                throw new NotImplementedException("Currently unable to register multiple hot keys");

            _onHotKeyPressed = onHotKeyPressed;
            RegisterHotKey(Handle, id, modifiers, keys);
        }

        #region DataStructures

        public struct GuiThreadInfo
        {
            public int cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndCapred;
            public IntPtr hwndCapture;
            public IntPtr hwndFocus;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public RECT rcCaret;
        };

        public struct RECT
        {
            public uint bottom;
            public uint left;
            public uint right;
            public uint top;
        }

        #endregion DataStructures
    }
}