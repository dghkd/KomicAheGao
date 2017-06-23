using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Diagnostics;

using KomicAheGao.Common;

namespace KomicAheGao
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex;

        public App()
            : base()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            String errorMessage = String.Format("因為這裡面有異音:\r\n {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool isNew;
            String strMuteId = "KomicAheGao_{57B300B2-DAF1-4C18-8A9C-30CC689DD6D0}";

            if (e.Args.Length > 0)
            {
                foreach (String arg in e.Args)
                {
                    if (arg == AppEnums.APP_ARG_AUTOSTART)
                    {
                        App.Current.Properties[AppEnums.APP_PROP_KEY_ISAUTOSTART] = true;
                    }
                }
            }

            // Create mutex
            _mutex = new Mutex(true, strMuteId, out isNew);

            if (!isNew)
            {
                Debug.WriteLine("KomicAheGao has opened");
                App.Current.Properties[AppEnums.APP_PROP_KEY_ISOPENED] = true;

                RegMethod.SetMessage(RegMethod.REG_Value_Open_Manage, true);
                Shutdown();
            }
            else
            {
                Debug.WriteLine("KomicAheGao Startup!");
                base.OnStartup(e);
            }

        }

        private void On_Application_Activated(object sender, EventArgs e)
        {
            App.Current.Properties[AppEnums.APP_PROP_KEY_ISACTIVATED] = true;
        }

        private void On_Application_Deactivated(object sender, EventArgs e)
        {
            App.Current.Properties[AppEnums.APP_PROP_KEY_ISACTIVATED] = false;
        }
    }
}
