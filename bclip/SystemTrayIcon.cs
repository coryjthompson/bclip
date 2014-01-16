using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using bclip.Model;

namespace bclip
{
    class SystemTrayIcon : IDisposable
    {
        private NotifyIcon notifyIcon;
        private ContextMenu copyItemMenu;
        private ContextMenu systemMenu;

        public SystemTrayIcon()
        {
            notifyIcon = new NotifyIcon();
        }

        public void Display()
        {
            notifyIcon.MouseDown += new MouseEventHandler(MouseDown);
            notifyIcon.Icon = new Icon("icon.ico");
            notifyIcon.Text = "System Tray Utility Application Demonstration Program";
            notifyIcon.Visible = true;

            CreateSystemMenu();
            PopulateCopyItemMenu(null);

            copyItemMenu = new ContextMenu();
            notifyIcon.ContextMenu = systemMenu;
        }

        public void Dispose()
        {
            notifyIcon.Dispose();
        }

        public void PopulateCopyItemMenu(List<CopyItem> copyItems)
        {
            if (copyItems == null || copyItems.Count <= 0)
                return;

            copyItemMenu.MenuItems.Clear();
            foreach(CopyItem copyItem in copyItems){
                copyItemMenu.MenuItems.Add(copyItem.Content);
            }

        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                notifyIcon.ContextMenu = copyItemMenu;
                ShowContextMenu(notifyIcon);
                notifyIcon.ContextMenu = systemMenu;
            }
        }

        private void ShowContextMenu(NotifyIcon notifyIcon)
        {
            MethodInfo methodInfo = typeof(NotifyIcon).GetMethod("ShowContextMenu", System.Reflection.BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(notifyIcon, null);
        }

        private void CreateSystemMenu()
        {
            systemMenu = new ContextMenu();

            MenuItem exitMenuItem = new MenuItem("Exit");
            exitMenuItem.Click += new EventHandler(OnExitClick);

            MenuItem aboutMenuItem = new MenuItem("About");
            aboutMenuItem.Click += new EventHandler(OnAboutClick);

            MenuItem launchOnStartupMenuItem = new MenuItem("Launch on startup");
            launchOnStartupMenuItem.Checked = true;
            launchOnStartupMenuItem.Click += new EventHandler(OnLaunchOnStartupClick);

            MenuItem plainTextMenuItem = new MenuItem("Convert to Plain Text");
            plainTextMenuItem.Checked = true;
            plainTextMenuItem.Click += new EventHandler(OnPlainTextClick);

            systemMenu.MenuItems.Add(launchOnStartupMenuItem);
            systemMenu.MenuItems.Add(plainTextMenuItem);
            systemMenu.MenuItems.Add("-");
            systemMenu.MenuItems.Add(aboutMenuItem);
            systemMenu.MenuItems.Add(exitMenuItem);

        }

        private void OnExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnAboutClick(object sender, EventArgs e)
        {
            MessageBox.Show("About");
        }

        private void OnPlainTextClick(object sender, EventArgs e)
        {
            MenuItem menuItem = systemMenu.MenuItems[1];
            menuItem.Checked = !menuItem.Checked;
        }

        private void OnLaunchOnStartupClick(object sender, EventArgs e)
        {
            MenuItem menuItem = systemMenu.MenuItems[0];
            menuItem.Checked = !menuItem.Checked;
        }
    }
}