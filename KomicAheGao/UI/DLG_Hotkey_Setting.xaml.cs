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
using System.Windows.Shapes;
using System.Diagnostics;

namespace KomicAheGao.UI
{
    /// <summary>
    /// DLG_Hotkey_Setting.xaml 的互動邏輯
    /// </summary>
    public partial class DLG_Hotkey_Setting : Window
    {
        private Win32Helper.KeyModifier _modifier;
        private Key _key;
        public DLG_Hotkey_Setting()
        {
            InitializeComponent();
            CMB_Modifier.Items.Add(Win32Helper.KeyModifier.Alt);
            CMB_Modifier.Items.Add(Win32Helper.KeyModifier.Control);
            CMB_Modifier.Items.Add(Win32Helper.KeyModifier.Shift);

            String curHotkey = HotKeyCtrl.Data.Modifier.ToString() + "+" + HotKeyCtrl.Data.VKey.ToString();
            TXTBOX_CurHotkey.Text = curHotkey;
        }

        private void On_TXTBOX_Hotkey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // The text box grabs all input.
            e.Handled = true;
            Debug.WriteLine(String.Format("{0} {1}", e.Key, e.Key.ToString()));
            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return;
            }
            
            TXTBOX_Hotkey.Text = e.Key.ToString();
            _key = e.Key;
        }

        private void On_CMB_Modifier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _modifier = (Win32Helper.KeyModifier)CMB_Modifier.SelectedItem;
        }

        private void On_BTN_Reg_Click(object sender, RoutedEventArgs e)
        {
            bool ret = HotKeyCtrl.Data.RegisterHotKey(_modifier, _key);
            if (ret)
            {
                String curHotkey = HotKeyCtrl.Data.Modifier.ToString() + "+" + HotKeyCtrl.Data.VKey.ToString();
                TXTBOX_CurHotkey.Text = curHotkey;
                MessageBox.Show("熱鍵設定成功");
                this.Close();
            }
            else
            {
                MessageBox.Show("熱鍵設定失敗");
            }
        }
    }
}
