using BingHomeDesktopBackground.Models;
using BingHomeDesktopBackground.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public string tempPath;
        public string sourcePath;

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
            Initialize();
            main = new MainViewVM();
            CurrentViewModel = main;
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            CloseSettingsCommand = new RelayCommand(CloseSettings);
        }

        private void Initialize()
        {
            tempPath = SettingsManager.settings.DefaultTempPath;
            sourcePath = SettingsManager.settings.DefaultSourcePath;
            SynchronizeTempFilesWithSourceFiles();
            SettingsManager.LoadedImages = LoadImagesFromTemp(tempPath);
            
        }

        public void SynchronizeTempFilesWithSourceFiles()
        {
            HashSet<string> sourceElements = new HashSet<string>();
            HashSet<string> tempElements = new HashSet<string>();
            List<string> tempFiles = new List<string>(Directory.GetFiles(tempPath));
            foreach (string tempElement in Directory.GetFiles(tempPath))
            {
                tempElements.Add(Path.GetFileNameWithoutExtension(tempElement));
            }
            foreach (string sourceElement in Directory.GetFiles(sourcePath))
            {
                if (CheckFileIsWallpaper(sourceElement))
                {
                    string fileName = Path.GetFileName(sourceElement);
                    sourceElements.Add(fileName);
                    if (!tempElements.Contains(fileName))
                    {
                        var newPath = Path.Combine(tempPath, fileName);
                        var newFile = Path.ChangeExtension(newPath, ".jpg");
                        if (!File.Exists(newFile))
                        {
                            File.Copy(sourceElement, newFile);
                        }
                    }
                }
            }
            foreach (string data in tempFiles)
            {
                if (!sourceElements.Contains(Path.GetFileNameWithoutExtension(data)))
                {
                    File.Delete(data); 
                }
            }
        }

        public bool CheckFileIsWallpaper(string path)
        {
            Bitmap img = new Bitmap(path);
            if (img.Width > 1000 && img.Height > 1000)
            {
                return true;
            }
            return false;
        }

        public ObservableCollection<ImageElement> LoadImagesFromTemp(string tempPath)
        {
            ObservableCollection<ImageElement> images = new ObservableCollection<ImageElement>();
            foreach (string data in Directory.GetFiles(tempPath))
            {
                BitmapImage background = new BitmapImage(new Uri(data, UriKind.Absolute));
                ImageElement newImage = new ImageElement();
                newImage.CurrentImage = background;
                newImage.Name = Path.GetFileNameWithoutExtension(new FileInfo(data).Name);
                newImage.CreationDate = new FileInfo(data).CreationTimeUtc;
                images.Add(newImage);
            }
            return images;
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
