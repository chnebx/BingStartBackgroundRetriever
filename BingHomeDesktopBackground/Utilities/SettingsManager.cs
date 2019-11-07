using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BingHomeDesktopBackground.Utilities
{
    public class SettingsManager
    {
        private IConfigurationRoot config;
        public SettingsManager()
        {
            config = new ConfigurationBuilder().AddJsonFile("Settings.json").Build();
            if (config["DefaultDestinationPath"] == "")
            {
                config["DefaultDestinationPath"] = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
        }

        public string GetDefaultDestinationPath()
        {
            return config["DefaultDestinationPath"];
        }

    }
}
