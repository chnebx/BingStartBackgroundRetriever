using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BingHomeDesktopBackground.ViewModels
{
    public class AppContainerViewVM: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        MainViewVM main = new MainViewVM();
        SettingsViewVM settings = new SettingsViewVM();

        private object _currentViewModel;

        public object CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentViewModel"));
            }
        }

        public RelayCommand OpenSettingsCommand { get; set; }
        public RelayCommand CloseSettingsCommand { get; set; }

        public AppContainerViewVM()
        {
            CurrentViewModel = main;
            OpenSettingsCommand = new RelayCommand(OpenSettings);
        }

        private void OpenSettings(object parameter)
        {
            CurrentViewModel = settings;
        }
    }
}
