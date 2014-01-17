using System;
using System.Windows.Forms;
using bclip.Model;

namespace bclip
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
