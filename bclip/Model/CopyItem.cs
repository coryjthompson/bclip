using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pasteboard.Model
{
    class CopyItem
    {
        public string Content;

        public static CopyItem FromClipboard(){
            CopyItem copyItem = new CopyItem();
            copyItem.Content = Clipboard.GetText();
            return copyItem;
        }
    }
}
