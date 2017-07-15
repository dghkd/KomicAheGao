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

using KomicAheGao.ViewModel;

namespace KomicAheGao.UI
{
    /// <summary>
    /// DLG_Manage.xaml 的互動邏輯
    /// </summary>
    public partial class DLG_Manage : Window
    {
        private GaoCollection _colle;

        public DLG_Manage()
        {
            InitializeComponent();
            _colle = GaoData.Data.GetCollection();
            LB_Gaos.ItemsSource = _colle;
            TXTBOX_ClipboardCount.Text = Convert.ToString(Properties.Settings.Default.ClipBoardCount);
        }

        private void On_BTN_Add_Click(object sender, RoutedEventArgs e)
        {
            GaoVM vm = new GaoVM();
            vm.Name = TXT_Name.Text;
            vm.Text = TXT_Text.Text;
            GaoData.Data.AddData(vm);
            _colle.Add(vm);
            TXT_Text.Clear();
            LB_Gaos.ScrollIntoView(vm);
        }

        private void On_BTN_MoveTop_Click(object sender, RoutedEventArgs e)
        {
            int idx = LB_Gaos.SelectedIndex;
            if (idx != -1)
            {
                _colle.Move(idx, 0);
                GaoData.Data.MoveTo(idx, 0);
                LB_Gaos.ScrollIntoView(LB_Gaos.SelectedItem);
            }
        }

        private void On_BTN_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            int idx = LB_Gaos.SelectedIndex;
            if (idx > 0)
            {
                _colle.Move(idx, idx - 1);
                GaoData.Data.MoveTo(idx, idx - 1);
                LB_Gaos.ScrollIntoView(LB_Gaos.SelectedItem);
            }
        }

        private void On_BTN_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            int idx = LB_Gaos.SelectedIndex;
            if (idx >= 0
                && idx < LB_Gaos.Items.Count - 1)
            {
                _colle.Move(idx, idx + 1);
                GaoData.Data.MoveTo(idx, idx + 1);
                LB_Gaos.ScrollIntoView(LB_Gaos.SelectedItem);
            }
        }

        private void On_BTN_MoveBottom_Click(object sender, RoutedEventArgs e)
        {
            int idx = LB_Gaos.SelectedIndex;
            if (idx >= 0
                && idx < LB_Gaos.Items.Count - 1)
            {
                _colle.Move(idx, LB_Gaos.Items.Count - 1);
                GaoData.Data.MoveTo(idx, LB_Gaos.Items.Count - 1);
                LB_Gaos.ScrollIntoView(LB_Gaos.SelectedItem);
            }
        }

        private void On_BTN_Del_Click(object sender, RoutedEventArgs e)
        {
            GaoVM vm = LB_Gaos.SelectedItem as GaoVM;
            if (vm != null)
            {
                int idx = LB_Gaos.SelectedIndex + 1;
                _colle.Remove(vm);
                GaoData.Data.DeleteData(vm);
                LB_Gaos.SelectedIndex = idx;
            }
        }

        private void On_BTN_Replace_Click(object sender, RoutedEventArgs e)
        {
            GaoVM vm = LB_Gaos.SelectedItem as GaoVM;
            if (vm != null)
            {
                vm.Name = TXT_Name.Text;
                vm.Text = TXT_Text.Text;
                GaoData.Data.SaveData();
            }
        }

        private void On_BTN_Insert_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Gaos.SelectedIndex != -1)
            {
                int idx = LB_Gaos.SelectedIndex + 1;
                GaoVM vm = new GaoVM();
                vm.Name = TXT_Name.Text;
                vm.Text = TXT_Text.Text;
                GaoData.Data.Insert(idx, vm);
                _colle.Insert(idx, vm);
                TXT_Text.Clear();
                LB_Gaos.ScrollIntoView(vm);
            }
        }

        private void On_BTN_Load_Click(object sender, RoutedEventArgs e)
        {
            GaoVM vm = LB_Gaos.SelectedItem as GaoVM;
            if (vm != null)
            {
                TXT_Name.Text = vm.Name;
                TXT_Text.Text = vm.Text;
            }
        }

        private void On_TXTBOX_ClipboardCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int count = 0;
            if (!Int32.TryParse(e.Text, out count))
            {
                e.Handled = true;
            }
        }

        private void On_TXTBOX_ClipboardCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int count = Properties.Settings.Default.ClipBoardCount;
            if (TXTBOX_ClipboardCount.Text == "")
            {
                TXTBOX_ClipboardCount.Text = Convert.ToString(count);
                return;
            }

            if (Int32.TryParse(TXTBOX_ClipboardCount.Text, out count))
            {
                Properties.Settings.Default.ClipBoardCount = count;
                Properties.Settings.Default.Save();
            }
        }
    }
}
