using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using IWshRuntimeLibrary;
using System.Reflection;

namespace KomicAheGao
{
    public class WinTray
    {
        #region Private Member
        private ContextMenu _contextMenu;
        #endregion

        #region Public Member
        public NotifyIcon TrayIcon;
        public MenuItem MenuItem_About, MenuItem_Exit, MenuItem_Manage, MenuItem_Hotkey, MenuItem_AutoStart;
        
        #endregion

        #region Constructor
        public WinTray(Stream stream)
        {
            Init();
            TrayIcon.Icon = new Icon(stream);
        }
        #endregion

        #region Private Function
        private void Init()
        {
            TrayIcon = new NotifyIcon();
            _contextMenu = new ContextMenu();
            this.MenuItem_About = new MenuItem("About");
            this.MenuItem_Manage = new MenuItem("Manage");
            this.MenuItem_Hotkey = new MenuItem("Hotkey Setting");
            this.MenuItem_Exit = new MenuItem("Exit");
            this.MenuItem_AutoStart = new MenuItem("Auto Start");

            // Initialize context menu
            _contextMenu.MenuItems.Add(this.MenuItem_Manage);
            _contextMenu.MenuItems.Add(this.MenuItem_Hotkey);
            _contextMenu.MenuItems.Add(this.MenuItem_AutoStart);
            _contextMenu.MenuItems.Add("-");
            _contextMenu.MenuItems.Add(this.MenuItem_About);
            _contextMenu.MenuItems.Add("-");
            _contextMenu.MenuItems.AddRange(new MenuItem[] { this.MenuItem_Exit });

            this.MenuItem_AutoStart.Checked = this.IsStartupLinkExist();
            this.MenuItem_AutoStart.Click += On_MenuItem_AutoStart_Click;

            TrayIcon.ContextMenu = this._contextMenu;

            TrayIcon.Visible = true;
        }


        private void On_MenuItem_AutoStart_Click(object sender, EventArgs e)
        {
            this.MenuItem_AutoStart.Checked = !this.MenuItem_AutoStart.Checked;
            this.EnableAutoStartup(this.MenuItem_AutoStart.Checked);
        }

        /// <summary>
        /// Create the Startup shortcut file.
        /// <para> In order to access the classes that will enable us to create shortcuts we need to add the Windows Script Host Object Model library as a reference to our project first. </para> 
        /// <para> 1.Right click on your project </para>
        /// <para> 2.Click “Add Reference…” </para>
        /// <para> 3.Select the “COM” tab on the left </para>
        /// <para> 4.Search for Windows Script Host Object Model and add it as a reference </para>
        /// </summary>
        private void CreateStartupLink()
        {
            String shortcutLocation = GetStartupLinkPath();
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            AssemblyDescriptionAttribute asmdc = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(System.Reflection.AssemblyDescriptionAttribute));
            shortcut.Description = asmdc.Description;
            shortcut.TargetPath = Assembly.GetExecutingAssembly().Location;
            shortcut.Arguments = AppEnums.APP_ARG_AUTOSTART;
            shortcut.Save();
        }

        /// <summary>
        /// Delete the Startup shortcut file.
        /// </summary>
        private void DeleteStartupLink()
        {
            String shortcutLocation = GetStartupLinkPath();
            System.IO.File.Delete(shortcutLocation);
        }

        /// <summary>
        /// Get full path of the Startup link file at SpecialFolder.
        /// </summary>
        private String GetStartupLinkPath()
        {
            String ret = "";
            String linkName = "KomicAheGao.lnk";
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            ret = System.IO.Path.Combine(path, linkName);
            return ret;
        }

        #endregion


        #region Public Method
        public bool IsStartupLinkExist()
        {
            String link = GetStartupLinkPath();
            return System.IO.File.Exists(link);
        }

        public void EnableAutoStartup(bool isEnable)
        {
            if (isEnable)
            {
                CreateStartupLink();
            }
            else
            {
                DeleteStartupLink();
            }
        }
        #endregion
    }
}
