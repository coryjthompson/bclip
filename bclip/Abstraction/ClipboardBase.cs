using System;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Pasteboard.Model;

namespace Pasteboard
{
    delegate void OnCopyDetectedDelegate(CopyItem copyItem);
    delegate void OnPasteDetectedDelegate();

    abstract class ClipboardBase:Form
    {
        public OnCopyDetectedDelegate OnCopyDetected;
        public OnPasteDetectedDelegate OnPasteDetected;

        public virtual CopyItem GetClipboardContent()
        {
            throw new NotImplementedException("GetClipboardContent must be implemented");
        }
    }
}
