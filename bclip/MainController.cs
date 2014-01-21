using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using bclip.Model;
using bclip.Windows;

namespace bclip
{
    internal class MainController
    {
        private const string OS_WINDOWS = "Windows";
        private const string OS_OSX = "OSX";

        public ClipboardBase ClipboardHandler;
        public List<CopyItem> CopyHistory;

        private readonly Stopwatch _stopwatch;
        private string _operatingSystem;

        private CopyItem _lastPaste;

        private int _pasteCount = 0;

        private SystemTrayIcon _systemTrayIcon;

        public MainController()
        {
            string operatingSystem = DetectOperatingSystem();

            if (operatingSystem == OS_WINDOWS)
                ClipboardHandler = new WindowsClipboard();
            else
                throw new NotImplementedException("Unknown Operating System");

            ClipboardHandler.OnCopyDetected = OnCopyDetected;
            ClipboardHandler.OnPasteDetected = OnPasteDetected;

            CopyHistory = new List<CopyItem>();
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            _lastPaste = null;
        }

        public void OnCopyDetected(CopyItem copyItem)
        {
            if (_lastPaste != null && _lastPaste.Content == copyItem.Content)
            {
                //lastPaste = null;
                return;
            }

            CopyHistory.Insert(0, copyItem);
            _systemTrayIcon.PopulateCopyItemMenu(CopyHistory);
        }

        public void SetSystemTrayIcon(SystemTrayIcon systemTrayIcon)
        {
            _systemTrayIcon = systemTrayIcon;
        }

        public void OnPasteDetected()
        {
            long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds < 1000)
            {
                _pasteCount++;
                WindowsClipboard.SendUndo();
            }
            else
            {
                _pasteCount = 0;
            }

            //If no copies have been saved, then do nothing
            if (CopyHistory.Count == 0)
                return;

            if (_pasteCount >= CopyHistory.Count)
                _pasteCount = 0;

            _lastPaste = CopyHistory[_pasteCount];
            Clipboard.SetText(_lastPaste.Content);

            _stopwatch.Restart();
            _stopwatch.Start();
        }

        private string DetectOperatingSystem()
        {
            return _operatingSystem ?? (_operatingSystem = OS_WINDOWS);
        }
    }
}