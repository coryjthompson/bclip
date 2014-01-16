using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using bclip.Model;

namespace bclip.Abstraction
{
    class WindowsClipboard:ClipboardBase
    {
        #region fields
        public static int KEY_V = 0x0056;
        public static int MOD_CONTROL = 0x0002;
        const int WM_PASTE = 0x302;

 
        #endregion
     
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll")]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove,IntPtr hWndNewNext);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam,IntPtr lParam);
        
        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll")]
        static extern bool GetGUIThreadInfo(uint idThread, ref GuiThreadInfo lpgui);


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int iLeft;
            public int iTop;
            public int iRight;
            public int iBottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct GuiThreadInfo
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rectCaret;
        }

        IntPtr nextClipboardViewer;

        public WindowsClipboard()
        {
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);

            RegisterHotKey((IntPtr)this.Handle, 1, MOD_CONTROL, KEY_V);
        }
        private void SendPaste()
        {
            IntPtr hWnd = GetFocusedHandle();
            PostMessage(hWnd, WM_PASTE, IntPtr.Zero, IntPtr.Zero);
        }

        public static IntPtr GetFocusedHandle()
        {
            var info = new GuiThreadInfo();
            info.cbSize = Marshal.SizeOf(info);
            if (!GetGUIThreadInfo(0, ref info))
                throw new Win32Exception();
            return info.hwndFocus;
        }


        protected override void Dispose(bool disposing)
        {
            ChangeClipboardChain(this.Handle, nextClipboardViewer);
            UnregisterHotKey((IntPtr)this.Handle, 1);
            base.Dispose(disposing);
        }
        
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;
            const int WM_HOTKEY = 0x312;

            switch (m.Msg)
            {
                case WM_HOTKEY:
                    this.OnPasteDetected();
                    SendPaste();
                    break;
                case WM_DRAWCLIPBOARD:
                    CopyItem copyItem = CopyItem.FromClipboard();
                    this.OnCopyDetected(copyItem);

                    SendMessage(nextClipboardViewer, m.Msg, m.WParam,m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
            
        }
    }
}