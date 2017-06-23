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

using KomicAheGao.ViewModel;

namespace KomicAheGao.UI
{
    /// <summary>
    /// UC_Clipboard_Item.xaml 的互動邏輯
    /// </summary>
    public partial class UC_Clipboard_Item : UserControl
    {
        public UC_Clipboard_Item()
        {
            InitializeComponent();
        }

        private void On_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClipboardVM vm = this.DataContext as ClipboardVM;
            if (vm != null)
            {
                vm.CmdSendData.Execute(null);
            }
        }
    }
}
