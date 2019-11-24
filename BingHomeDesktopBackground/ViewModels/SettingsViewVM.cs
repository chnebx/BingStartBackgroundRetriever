using BingHomeDesktopBackground.Utilities;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;

namespace BingHomeDesktopBackground.ViewModels
{
    public class SettingsViewVM: INotifyPropertyChanged
    {
        private string _sourcePath;
        private string _tempPath;
        private bool _isModified;
        private string _lastSavedSourcePath;
        private string _lastSavedTempPath;
        private string _shortSourcePath;
        public event PropertyChangedEventHandler PropertyChanged;

        

        public string ShortSourcePath
        {
            get 
            { 
                return _shortSourcePath;
            }
            set 
            { 
                _shortSourcePath = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShortSourcePath"));
            }
        }

        private string _shortTempPath;

        public string ShortTempPath
        {
            get 
            { 
                return _shortTempPath; 
            }
            set 
            { 
                _shortTempPath = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShortTempPath"));
            }
        }


        public string SourcePath
        {
            get
            {
                return _sourcePath;
            }
            set
            {
                _sourcePath = value;
                ShortSourcePath = ShortenPath(_sourcePath);
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
                ShortTempPath = ShortenPath(_tempPath);
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
            _lastSavedSourcePath = SourcePath;
            _lastSavedTempPath = TempPath;
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
            SaveLatestPathsSettings();
        }

        private string ShortenPath(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && path.Length > 50)
            {
                string driveLetter = path.Substring(0, 30);
                string[] directory = path.Split(@"\");
                string lastDirectory = directory[directory.Length - 1];
                if (lastDirectory.Length > 10)
                {
                    lastDirectory = $"{lastDirectory.Substring(0, 15)}...";
                }
                return $"{driveLetter}...\\{lastDirectory}";
            } else
            {
                return path;
            }
        }

        private void SaveLatestPathsSettings()
        {
            _lastSavedSourcePath = SourcePath;
            _lastSavedTempPath = TempPath;
        }

        private void SaveSettings(object parameter)
        {
            if (!Directory.Exists(SourcePath))
            {
                MessageBox.Show("The source folder path you defined is not valid", "Path error", MessageBoxButton.OK, MessageBoxImage.Warning);
                SourcePath = _lastSavedSourcePath;
            }
            if (!Directory.Exists(TempPath))
            {
                MessageBox.Show("The temp folder path you defined is not valid", "Path error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TempPath = _lastSavedTempPath;
            }
            SettingsManager.SaveSettingsPaths(SourcePath, TempPath);
            SaveLatestPathsSettings();
            IsModified = false;
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
