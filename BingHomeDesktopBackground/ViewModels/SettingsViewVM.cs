using System;
using System.Collections.Generic;
using System.Text;

namespace BingHomeDesktopBackground.ViewModels
{
    public class SettingsViewVM
    {
        private string _destinationPath;
        public string DestinationPath
        {
            get
            {
                return _destinationPath;
            }
            set
            {
                _destinationPath = value;
            }
        }

        public SettingsViewVM()
        {

        }
    }
}
