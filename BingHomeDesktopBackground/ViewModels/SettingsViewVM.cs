using BingHomeDesktopBackground.Utilities;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BingHomeDesktopBackground.ViewModels
{
    public class SettingsViewVM: INotifyPropertyChanged
    {
        private string _sourcePath;
        private string _tempPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public string SourcePath
        {
            get
            {
                return _sourcePath;
            }
            set
            {
                _sourcePath = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DestinationPath"));
            }
        }

        public string TempPath
        {
            get
            {
                return _tempPath;
            }
            set
            {
                _tempPath = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("TempPath"));
            }
        }

        public RelayCommand SaveSettingsCommand { get; set; }
        public RelayCommand SelectSourcePathCommand { get; set; }
        public RelayCommand SelectTempPathCommand { get; set; }

        public SettingsViewVM()
        {
            SourcePath = SettingsManager.settings.DefaultSourcePath;
            TempPath = SettingsManager.settings.DefaultTempPath;
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            SelectSourcePathCommand = new RelayCommand(SelectSourcePath);
            SelectTempPathCommand = new RelayCommand(SelectTempPath);
        }

        private void SaveSettings(object parameter)
        {
            SettingsManager.SaveSettingsPaths(SourcePath, TempPath);
        }

        private void SelectSourcePath(object parameter)
        {
            string sourcePath = GetDialogPath(SourcePath);
            if (sourcePath != null)
            {
                SourcePath = sourcePath;
            }
            
        }

        private void SelectTempPath(object parameter)
        {
            string tempPath = GetDialogPath(TempPath);
            if (tempPath != null)
            {
                TempPath = tempPath;
            }
        }

        private string GetDialogPath(string path)
        {
            return FilesManager.OpenFolderDialog(path);
        }
    }

}
