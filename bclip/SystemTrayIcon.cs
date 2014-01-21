using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using bclip.Model;
using bclip.Properties;

namespace bclip
{
    class SystemTrayIcon : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        
        private ContextMenu _copyItemMenu;
        private ContextMenu _systemMenu;

        public SystemTrayIcon()
        {
            _notifyIcon = new NotifyIcon();
        }

        public void Display()
        {
            _notifyIcon.MouseDown += new MouseEventHandler(MouseDown);
            _notifyIcon.Icon = new Icon("icon.ico");
            _notifyIcon.Text = Resources.SystemTrayIcon_Display;
            _notifyIcon.Visible = true;

            Create_systemMenu();
            PopulateCopyItemMenu(null);

            _copyItemMenu = new ContextMenu();
            _notifyIcon.ContextMenu = _systemMenu;
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }

        public void PopulateCopyItemMenu(List<CopyItem> copyItems)
        {
            if (copyItems == null || copyItems.Count <= 0)
                return;

            _copyItemMenu.MenuItems.Clear();
            foreach(CopyItem copyItem in copyItems){
                _copyItemMenu.MenuItems.Add(copyItem.Content);
            }

        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _notifyIcon.ContextMenu = _copyItemMenu;
                ShowContextMenu(_notifyIcon);
                _notifyIcon.ContextMenu = _systemMenu;
            }
        }

        private void ShowContextMenu(NotifyIcon notifyIcon)
        {
            MethodInfo methodInfo = typeof(NotifyIcon).GetMethod("ShowContextMenu", System.Reflection.BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(notifyIcon, null);
        }

        private void Create_systemMenu()
        {
            _systemMenu = new ContextMenu();

            MenuItem exitMenuItem = new MenuItem("Exit");
            exitMenuItem.Click += OnExitClick;

            MenuItem aboutMenuItem = new MenuItem(Resources.SystemTrayIcon_OnAboutClick);
            aboutMenuItem.Click += OnAboutClick;

            MenuItem launchOnStartupMenuItem = new MenuItem("Launch on startup");
            launchOnStartupMenuItem.Checked = true;
            launchOnStartupMenuItem.Click += OnLaunchOnStartupClick;

            MenuItem plainTextMenuItem = new MenuItem("Convert to Plain Text");
            plainTextMenuItem.Checked = true;
            plainTextMenuItem.Click += OnPlainTextClick;

            _systemMenu.MenuItems.Add(launchOnStartupMenuItem);
            _systemMenu.MenuItems.Add(plainTextMenuItem);
            _systemMenu.MenuItems.Add("-");
            _systemMenu.MenuItems.Add(aboutMenuItem);
            _systemMenu.MenuItems.Add(exitMenuItem);

        }

        private void OnExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnAboutClick(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.SystemTrayIcon_OnAboutClick);
        }

        private void OnPlainTextClick(object sender, EventArgs e)
        {
            MenuItem menuItem = _systemMenu.MenuItems[1];
            menuItem.Checked = !menuItem.Checked;
        }

        private void OnLaunchOnStartupClick(object sender, EventArgs e)
        {
            MenuItem menuItem = _systemMenu.MenuItems[0];
            menuItem.Checked = !menuItem.Checked;
        }
    }
}