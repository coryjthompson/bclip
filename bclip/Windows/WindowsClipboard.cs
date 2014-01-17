using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using bclip.Model;
using bclip.Windows;

namespace bclip.Windows
 
{
    class WindowsClipboard:ClipboardBase
    {
        #region fields
        public static int KEY_V = 0x0056;
        public static int MOD_CONTROL = 0x0002;
        #endregion
     
        

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
        static extern bool GetGUIThreadInfo(uint idThread, ref User32Wrapper lpgui);
        IntPtr nextClipboardViewer;
        private User32Wrapper.GuiThreadInfo guiInfo;

        public WindowsClipboard()
        {
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            User32Wrapper user32Wrapper = new User32Wrapper();
            user32Wrapper.RegisterHotKey(1, MOD_CONTROL, KEY_V, OnHotKeyPressed);
        }

        public void OnHotKeyPressed(object sender, EventArgs e)
        {
            OnPasteDetected();
        }
   

        protected override void Dispose(bool disposing)
        {
            ChangeClipboardChain(this.Handle, nextClipboardViewer);
            base.Dispose(disposing);
        }
        
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
       
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