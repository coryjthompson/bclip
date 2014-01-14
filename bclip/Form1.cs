using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pasteboard
{

    public partial class Form1 : Form
    {
        private MainController _MainController;

        public Form1()
        {
            InitializeComponent();
            this._MainController = new MainController();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
