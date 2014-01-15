using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bclip.Abstraction;
using bclip.Model;

namespace bclip
{
    class MainController
    {
        private const string OS_WINDOWS = "Windows";
        private const string OS_OSX = "OSX";

        public ClipboardBase ClipboardHandler;
        public List<CopyItem> CopyHistory;

        private Stopwatch _stopwatch;
        private string _operatingSystem;

        private bool isClipboardLocked;

        private int pasteCount = 0;

        private SystemTrayIcon systemTrayIcon;

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

            this.isClipboardLocked = false;
        }

        public void OnCopyDetected(CopyItem copyItem)
        {
            if (isClipboardLocked)
            {
                isClipboardLocked = false;
                return;
            }

            CopyHistory.Insert(0, copyItem);
            systemTrayIcon.PopulateCopyItemMenu(CopyHistory);
        }

        public void setSystemTrayIcon(SystemTrayIcon systemTrayIcon)
        {
            this.systemTrayIcon = systemTrayIcon;
        }

        public void OnPasteDetected()
        {
            long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            
            if (elapsedMilliseconds < 2000)
            {
                pasteCount++;

                if (pasteCount > 1)
                {
                    int backspaceLength = CopyHistory[pasteCount - 1].Content.Length;

                    for (int i = 0; i < backspaceLength; i++)
                    {
                        SendKeys.Send("{BACKSPACE}");
                    }
                }

                //loop around if reached limit
                if (pasteCount > CopyHistory.Count)
                {
                    pasteCount = 0;
                }

            } else {
                pasteCount = 0;
            }

            isClipboardLocked = true;
            Clipboard.SetText(CopyHistory[pasteCount].Content);
            
            _stopwatch.Restart();
            _stopwatch.Start();
        }

        private string DetectOperatingSystem()
        {
            return this._operatingSystem ?? (this._operatingSystem = OS_WINDOWS);
        }
    }
}
