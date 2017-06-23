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
using System.Reflection;
using System.Diagnostics;

namespace KomicAheGao.UI
{
    /// <summary>
    /// DLG_About.xaml 的互動邏輯
    /// </summary>
    public partial class DLG_About : Window
    {
        private int _clickCount = 0;

        public DLG_About()
        {
            InitializeComponent();

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            
            TXT_Production.Text = String.Format("{0}", versionInfo.ProductName);
            TXT_Version.Text = String.Format("({0} {1})", "版本:", versionInfo.ProductVersion);
            TXT_Copyright.Text = versionInfo.LegalCopyright;
        }

        private void On_Viewbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _clickCount++;
            if (_clickCount % 10 == 0 && _clickCount < 50)
            {
                MessageBox.Show("裡面什麼都沒有喔", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (_clickCount == 50)
            {
                MessageBox.Show("中に誰もいませんよ～♪", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                _clickCount = 0;
            }
        }
    }
}
