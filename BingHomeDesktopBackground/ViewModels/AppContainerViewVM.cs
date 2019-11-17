using BingHomeDesktopBackground.Models;
using BingHomeDesktopBackground.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace BingHomeDesktopBackground.ViewModels
{
    public class AppContainerViewVM: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        MainViewVM main;
        SettingsViewVM settings = new SettingsViewVM();
        public static string tempPath;
        public static string sourcePath;

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
            main = new MainViewVM();
            CurrentViewModel = main;
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            CloseSettingsCommand = new RelayCommand(CloseSettings);
        }

        private void CloseSettings(object obj)
        {
            CurrentViewModel = main;
        }

        private void OpenSettings(object parameter)
        {
            CurrentViewModel = settings;
        }
    }
}
