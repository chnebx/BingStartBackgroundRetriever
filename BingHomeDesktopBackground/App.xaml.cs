using BingHomeDesktopBackground.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BingHomeDesktopBackground
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SettingsManager.Init();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SettingsManager.SaveSettings();
        }
    }
}
