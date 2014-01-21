using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using bclip.Model;
using bclip.Windows;

namespace bclip
{
    class MainController
    {
        private const string OS_WINDOWS = "Windows";
        private const string OS_OSX = "OSX";

        public ClipboardBase ClipboardHandler;
        public List<CopyItem> CopyHistory;

        private readonly Stopwatch _stopwatch;
        private string _operatingSystem;

        private CopyItem lastPaste;

        private int pasteCount = 0;

        private SystemTrayIcon SystemTrayIcon;

        public MainController()
        {
            string operatingSystem = DetectOperatingSystem();
            
            if (operatingSystem == OS_WINDOWS)
                this.ClipboardHandler = new WindowsClipboard();
            else
                throw new NotImplementedException("Unknown Operating System");

            this.ClipboardHandler.OnCopyDetected = this.OnCopyDetected;
            this.ClipboardHandler.OnPasteDetected = this.OnPasteDetected;

            this.CopyHistory = new List<CopyItem>();
            this._stopwatch = new Stopwatch();
            this._stopwatch.Start();

            lastPaste = null;
        }

        public void OnCopyDetected(CopyItem copyItem)
        {
            if (lastPaste != null && lastPaste.Content == copyItem.Content)
            {
                //lastPaste = null;
                return;
            }

            CopyHistory.Insert(0, copyItem);
            SystemTrayIcon.PopulateCopyItemMenu(CopyHistory);
        }

        public void SetSystemTrayIcon(SystemTrayIcon systemTrayIcon)
        {
            this.SystemTrayIcon = systemTrayIcon;
        }

        public void OnPasteDetected()
        {
            long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds < 1000)
            {
                pasteCount++;
                WindowsClipboard.SendUndo();
     
            } else {
                pasteCount = 0;
            }

            //If no copies have been saved, then do nothing
            if (CopyHistory.Count == 0)
                return;

            if (pasteCount >= CopyHistory.Count)
                pasteCount = 0;

            lastPaste = CopyHistory[pasteCount];
            Clipboard.SetText(lastPaste.Content);
            
            _stopwatch.Restart();
            _stopwatch.Start();
        }

        private string DetectOperatingSystem()
        {
            return this._operatingSystem ?? (this._operatingSystem = OS_WINDOWS);
        }
    }
}
