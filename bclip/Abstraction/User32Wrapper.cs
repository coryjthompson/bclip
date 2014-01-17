﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace bclip.Abstraction
{
    internal sealed class User32Wrapper:Form
    {
        private const int WM_GETTEXT = 0x0D;
        private const int WM_SETTEXT = 0x000c;
     
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public struct RECT
        {
            public uint left;
            public uint top;
            public uint right;
            public uint bottom;
        }

        public struct GuiThreadInfo
        {
            public int cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCapred;
            public RECT rcCaret;
        };

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", EntryPoint = "GetGUIThreadInfo")]
        private static extern bool GetGUIThreadInfo(int tId, out GuiThreadInfo threadInfo);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, System.Text.StringBuilder lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, string lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private EventHandler OnHotKeyPressed;

        private static GuiThreadInfo GetThreadInfo(int tid)
        {
            GuiThreadInfo tinfo = new GuiThreadInfo();
            tinfo.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(tinfo);
            GetGUIThreadInfo(tid, out tinfo);
            return tinfo;
        }

        public static string FocusedClassName()
        {
            GuiThreadInfo info = GetThreadInfo((int) 0);
            StringBuilder className = new StringBuilder(100);
            var x = GetClassName(info.hwndCapred, className, 100);
            return className.ToString();

        }

        public static IntPtr FocusedHandle()
        {
            GuiThreadInfo info = GetThreadInfo((int) 0);
            return info.hwndFocus;

        }

        public static string GetText()
        {
            IntPtr focusedHandle = FocusedHandle();
            System.Text.StringBuilder sb = new System.Text.StringBuilder(255);  // or length from call with GETTEXTLENGTH

            SendMessage(focusedHandle, WM_GETTEXT, sb.Capacity, sb);
            return sb.ToString();

        }

        public static void SetText()
        {
            IntPtr focusedHandle = FocusedHandle();
            StringBuilder sb = new StringBuilder(255);
            //sb.Append("Hello there");
            SendMessage(focusedHandle, WM_SETTEXT, IntPtr.Zero, "Hello World");
        }

        public new void Dispose()
        {
            UnregisterHotKey(this.Handle, 1);
            base.Dispose();
            //UnRegister hot key here.   
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

                    this.OnHotKeyPressed(this, new EventArgs());
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        public void RegisterHotKey(int id,  int modifiers, int keys, EventHandler onHotKeyPressed)
        {
            if(OnHotKeyPressed != null)
                throw new NotImplementedException("Currently unable to register multiple hot keys");

            OnHotKeyPressed = onHotKeyPressed;
            RegisterHotKey(this.Handle, id, modifiers, keys);
        }

    }
}