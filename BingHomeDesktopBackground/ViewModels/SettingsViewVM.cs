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
        private bool _isModified;

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
                if (!IsModified)
                {
                    IsModified = true;
                }
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
                if (!IsModified)
                {
                    IsModified = true;
                }
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("TempPath"));
            }
        }

        public bool IsModified
        {
            get 
            { 
                return _isModified; 
            }
            set 
            { 
                _isModified = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsModified"));
            }
        }


        public RelayCommand SaveSettingsCommand { get; set; }
        public RelayCommand SelectSourcePathCommand { get; set; }
        public RelayCommand SelectTempPathCommand { get; set; }
        public RelayCommand RestoreDefaultCommand { get; set; }

        public SettingsViewVM()
        {
            SourcePath = SettingsManager.settings.DefaultSourcePath;
            TempPath = SettingsManager.settings.DefaultTempPath;
            IsModified = false;
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            SelectSourcePathCommand = new RelayCommand(SelectSourcePath);
            SelectTempPathCommand = new RelayCommand(SelectTempPath);
            RestoreDefaultCommand = new RelayCommand(RestoreDefault);
        }

        private void RestoreDefault(object obj)
        {
            SourcePath = SettingsManager.BuildImagesSourcePath();
            TempPath = SettingsManager.BuildTempFolderPath();
            SettingsManager.SaveSettings();
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
