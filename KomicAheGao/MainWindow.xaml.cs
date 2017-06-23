using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Text.RegularExpressions;
using System.Reflection;

using KomicAheGao.UI;
using KomicAheGao.ViewModel;
using KomicAheGao.Common;

namespace KomicAheGao
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Member
        private HwndSource _hwndSrc;
        private WinTray _winTray;
        private GaoCollection _gaoColle;
        private ClipboardDataCollection _clipColle;
        private bool _shutdown;
        private bool _gaoPanel;
        private bool _skipClipUpdate;
        private System.Threading.Timer _skipClipTimer;
        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            _shutdown = false;


            //Init skip clipboard update event flag and timer.
            _skipClipUpdate = false;
            _skipClipTimer = new System.Threading.Timer(On_SkipClipboard_Update_TimerCallback);
            _skipClipTimer.Change(Timeout.Infinite, Timeout.Infinite);

            //Init Gao data.
            GaoData.Data.LoadData();
            GaoData.Data.CommandAction = On_GaoVM_CommandAction_Execute;
            _gaoPanel = true;

            //Init Clipboard collection.
            _clipColle = new ClipboardDataCollection();
            LB_ClipItems.ItemsSource = _clipColle;

            //Show the manage dialog if this app is not start by windows startup.
            bool isAutoStart = Convert.ToBoolean(App.Current.Properties[AppEnums.APP_PROP_KEY_ISAUTOSTART]);
            if (!isAutoStart)
            {
                DLG_Manage dlg = new DLG_Manage();
                dlg.Show();
            }
        }


        #endregion

        #region Windows Event

        private void On_Window_SourceInitialized(object sender, EventArgs e)
        {
            _hwndSrc = PresentationSource.FromVisual(this) as HwndSource;

            //Install windows message process hook.
            _hwndSrc.AddHook(WndProc);

            //Register global hotkey.
            HotKeyCtrl.Data.Init(_hwndSrc.Handle);
            HotKeyCtrl.Data.LoadHotKey();

            //Monitor windows clipboard. See WM_CLIPBOARDUPDATE event on WndProc.
            Win32Helper.AddClipboardFormatListener(_hwndSrc.Handle);

            this.Visibility = Visibility.Hidden;
        }

        private void On_Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTrayIcon();
            BeginRegMessageHandle();
        }

        private void BeginRegMessageHandle()
        {
            Task.Factory.StartNew(() =>
            {
                while (!_shutdown)
                {
                    object msg = RegMethod.GetMessage(RegMethod.REG_Value_Open_Manage);
                    if (msg != null)
                    {
                        bool isOpen = Convert.ToBoolean(msg);
                        if (isOpen)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                DLG_Manage dlg = new DLG_Manage();
                                dlg.Show();
                            });
                        }
                    }

                    SpinWait.SpinUntil(() => false, 100);
                }
            });
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32Helper.WM_NCLBUTTONDOWN:
                    this.Activate();
                    break;

                case Win32Helper.WM_EXITSIZEMOVE:
                    Deactivate();
                    break;

                case Win32Helper.WM_HOTKEY:
                    if (this.IsVisible)
                    {
                        this.Hide();
                    }
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    this.Activate();
                    break;

                case Win32Helper.WM_CLIPBOARDUPDATE:
                    On_Clipboard_Update();
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private void On_Clipboard_Update()
        {
            if (_skipClipUpdate == false)
            {
                SkipClipboardUpdate(500);
                try
                {
                    ClipboardVM vm = new ClipboardVM(System.Windows.Forms.Clipboard.GetDataObject());
                    if (vm != null)
                    {
                        vm.CommandAction += On_ClipboardVM_CommandAction_Execute;
                        _clipColle.Insert(0, vm);
                        while (_clipColle.Count > 100)
                        {
                            _clipColle.RemoveAt(100);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        private void On_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.IsActive)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                On_MenuItem_Exit_Click(null, null);
            }
        }

        private void On_Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
                PresentationSource source = PresentationSource.FromVisual(this);
                Screen screen = Screen.PrimaryScreen;
                double maxTop = screen.WorkingArea.Height / source.CompositionTarget.TransformToDevice.M22 - this.MaxHeight;
                double maxLeft = screen.WorkingArea.Width / source.CompositionTarget.TransformToDevice.M11 - this.Width;
                if (point.Y > maxTop)
                {
                    this.Top = maxTop;
                }
                else
                {
                    this.Top = point.Y / source.CompositionTarget.TransformToDevice.M22;
                }
                if (point.X > maxLeft)
                {
                    this.Left = maxLeft;
                }
                else
                {
                    this.Left = point.X / source.CompositionTarget.TransformToDevice.M11;
                }

                TXTBOX_Search.Text = "";

                _gaoColle = GaoData.Data.GetCollection();
                LB_Gaos.ItemsSource = null;
                LB_Gaos.ItemsSource = _gaoColle;

                LB_Gaos.SelectedIndex = 0;
                LB_Gaos.ScrollIntoView(LB_Gaos.SelectedItem);


                LB_ClipItems.SelectedIndex = 0;
                LB_ClipItems.ScrollIntoView(LB_ClipItems.SelectedItem);
            }
        }

        private void On_Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            else if (e.Key == Key.Enter)
            {
                if (_gaoPanel)
                {
                    if (LB_Gaos.SelectedItem != null)
                    {
                        GaoVM vm = LB_Gaos.SelectedItem as GaoVM;
                        vm.CmdSendText.Execute(null);
                    }
                }
                else
                {
                    if (LB_ClipItems.SelectedItem != null)
                    {
                        ClipboardVM vm = LB_ClipItems.SelectedItem as ClipboardVM;
                        vm.CmdSendData.Execute(null);
                    }
                }
            }
            else if (e.Key == Key.Up
                || e.Key == Key.Down)
            {
                if (_gaoPanel)
                {
                    int idx = LB_Gaos.SelectedIndex + (e.Key == Key.Up ? -1 : 1);
                    if (idx >= 0
                        && idx <= LB_Gaos.Items.Count)
                    {
                        LB_Gaos.SelectedIndex = idx;
                        LB_Gaos.ScrollIntoView(LB_Gaos.SelectedItem);
                    }
                }
                else
                {
                    int idx = LB_ClipItems.SelectedIndex + (e.Key == Key.Up ? -1 : 1);
                    if (idx >= 0
                        && idx <= LB_ClipItems.Items.Count)
                    {
                        LB_ClipItems.SelectedIndex = idx;
                        LB_ClipItems.ScrollIntoView(LB_ClipItems.SelectedItem);
                    }
                }
            }
            else if (e.Key == Key.Left
                || e.Key == Key.Right)
            {
                _gaoPanel = (e.Key == Key.Left);
                if (_gaoPanel)
                {
                    LB_Gaos.Visibility = Visibility.Visible;
                    LB_ClipItems.Visibility = Visibility.Collapsed;
                }
                else
                {
                    LB_Gaos.Visibility = Visibility.Collapsed;
                    LB_ClipItems.Visibility = Visibility.Visible;
                }
            }
            else
            {
                TXTBOX_Search.Focus();
            }
        }
        #endregion

        #region WinTray
        private void InitTrayIcon()
        {
            _winTray = new WinTray(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Icon/icon_app.ico")).Stream);
            _winTray.MenuItem_About.Click += On_MenuItem_About_Click;
            _winTray.MenuItem_Manage.Click += On_MenuItem_Manage_Click;
            _winTray.MenuItem_Hotkey.Click += On_MenuItem_Hotkey_Click;
            _winTray.MenuItem_Exit.Click += On_MenuItem_Exit_Click;
            _winTray.TrayIcon.DoubleClick += On_TrayIcon_DoubleClick;

            _winTray.MenuItem_About.Text = "關於";
            _winTray.MenuItem_Manage.Text = "管理";
            _winTray.MenuItem_Hotkey.Text = "熱鍵設定";
            _winTray.MenuItem_Exit.Text = "結束";
            _winTray.MenuItem_AutoStart.Text = "開機後啟動";
        }


        private void On_TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            DLG_Manage dlg = new DLG_Manage();
            dlg.Show();
        }

        private void On_MenuItem_About_Click(object sender, EventArgs e)
        {
            DLG_About dlg = new DLG_About();
            dlg.Show();
        }

        private void On_MenuItem_Manage_Click(object sender, EventArgs e)
        {
            DLG_Manage dlg = new DLG_Manage();
            dlg.Show();
        }

        private void On_MenuItem_Hotkey_Click(object sender, EventArgs e)
        {
            DLG_Hotkey_Setting dlg = new DLG_Hotkey_Setting();
            dlg.Show();
        }

        private void On_MenuItem_Exit_Click(object sender, EventArgs e)
        {
            if (_winTray != null)
            {
                _winTray.TrayIcon.Dispose();
            }

            if (_hwndSrc != null)
            {
                bool ret = Win32Helper.UnregisterHotKey(_hwndSrc.Handle, 1);
            }

            //Remove Clipboard listener.
            Win32Helper.RemoveClipboardFormatListener(_hwndSrc.Handle);

            _shutdown = true;
            System.Windows.Application.Current.Shutdown();
        }

        #endregion

        #region Control Event

        private void On_TXTBOX_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            _gaoColle = GaoData.Data.GetCollection(TXTBOX_Search.Text);
            LB_Gaos.ItemsSource = null;
            LB_Gaos.ItemsSource = _gaoColle;
        }

        private bool On_GaoVM_CommandAction_Execute(String cmdKey, GaoVM vm)
        {
            if (cmdKey == GaoVM.CmdKey_SendText)
            {
                SendText(vm);
                this.Hide();
                return true;
            }
            return false;
        }

        private bool On_ClipboardVM_CommandAction_Execute(String cmdKey, ClipboardVM vm)
        {
            if (cmdKey == ClipboardVM.CmdKey_SendData)
            {
                SendData(vm);
                this.Hide();
                return true;
            }
            return false;
        }

        private void On_SkipClipboard_Update_TimerCallback(object state)
        {
            _skipClipUpdate = false;
        }

        #endregion

        #region Private Method

        private void SendText(GaoVM vm)
        {
            //Save current clipboard data.
            Dictionary<String, Object> dict = new Dictionary<String, Object>();
            var dataObject = System.Windows.Forms.Clipboard.GetDataObject();
            foreach (var format in dataObject.GetFormats())
            {
                dict.Add(format, dataObject.GetData(format));
            }

            bool isAppActivated = Convert.ToBoolean(App.Current.Properties[AppEnums.APP_PROP_KEY_ISACTIVATED]);
            if (isAppActivated)
            {
                this.Hide();
            }

            //Change the clipboard data to Gao text.
            String goaText = "";
            int count = 0;

            do
            {
                try
                {
                    SkipClipboardUpdate(100);
                    System.Windows.Forms.Clipboard.SetText(vm.Text);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                goaText = System.Windows.Forms.Clipboard.GetText();
                SpinWait.SpinUntil(() => false, 10);
            } while (goaText != vm.Text && count < 100);

            //Send Gao text.
            SendKeys.SendWait("^{v}");
            SpinWait.SpinUntil(() => false, 100);

            //Restore clipboard data.
            System.Windows.Forms.DataObject obj = new System.Windows.Forms.DataObject();
            foreach (String format in dict.Keys)
            {
                obj.SetData(format, dict[format]);
            }

            //Restore the clipboard data.
            bool bRestore = false;
            for (int i = 0; i < 100 && !bRestore; i++)
            {
                try
                {
                    SkipClipboardUpdate(100);
                    System.Windows.Forms.Clipboard.SetDataObject(obj);
                    bRestore = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    bRestore = false;
                }
            }
        }

        private void SendData(ClipboardVM vm)
        {
            bool isAppActivated = Convert.ToBoolean(App.Current.Properties[AppEnums.APP_PROP_KEY_ISACTIVATED]);
            if (isAppActivated)
            {
                this.Hide();
            }

            //Set the clipboard data.
            bool bSet = false;
            for (int i = 0; i < 100 && !bSet; i++)
            {
                try
                {
                    SkipClipboardUpdate(100);
                    System.Windows.Forms.Clipboard.SetDataObject(vm.GetDataObject());
                    bSet = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    bSet = false;
                }
            }

            //Send data.
            SendKeys.SendWait("^{v}");
        }

        private void Deactivate()
        {
            this.Topmost = false;
            IntPtr targetProcess = Win32Helper.GetWindow(Process.GetCurrentProcess().MainWindowHandle, Win32Helper.GW_HWNDNEXT);
            IntPtr parentProces = Win32Helper.GetParent(targetProcess);
            while (parentProces != IntPtr.Zero)
            {
                targetProcess = parentProces;
                parentProces = Win32Helper.GetParent(targetProcess);
            }
            Win32Helper.SetForegroundWindow(targetProcess);

            this.Topmost = true;
        }

        /// <summary>
        /// Skip the clipboard update event for a period of time.
        /// </summary>
        /// <param name="millisecond">Millisecond</param>
        private void SkipClipboardUpdate(int millisecond)
        {
            _skipClipUpdate = true;
            _skipClipTimer.Change(millisecond, Timeout.Infinite);
        }
        #endregion

    }
}
