using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mime;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using BingHomeDesktopBackground.Models;
using System.Windows.Controls;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Diagnostics;
using BingHomeDesktopBackground.Utilities;
using System.Windows.Controls.Primitives;
using System.Threading;
using BingHomeDesktopBackground.Dialogs;

namespace BingHomeDesktopBackground.ViewModels
{
    public class MainWindowVM: INotifyPropertyChanged
    {
        private ObservableCollection<ImageElement> _images = new ObservableCollection<ImageElement>();
        private ObservableCollection<ImageElement> _selectedImages = new ObservableCollection<ImageElement>();
        private ObservableCollection<string> _results;
        private string tempPath;
        private string _shortDestinationPathName;
        private ICollectionView _imagesView;
        private string _destinationPath;
        private string sourcePath { get; set; }

        public ICollectionView ImagesView
        {
            get
            {
                return _imagesView;
            }
            set
            {
                _imagesView = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ImagesView"));
            }
        }

        public ObservableCollection<ImageElement> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Images"));
            }
        }

        public ObservableCollection<ImageElement> SelectedImages
        {
            get
            {
                return _selectedImages;
            }
            set
            {
                _selectedImages = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SelectedImages"));
            }
        }

        public ObservableCollection<string> Results
        {
            get
            {
                return _results;
            }
            set
            {
                _results = value;

            }
        }


        public string ShortDestinationPathName
        {
            get
            {
                if (_shortDestinationPathName == null)
                {
                    return "Undefined";
                }
                return _shortDestinationPathName;

            }
            set
            {
                _shortDestinationPathName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShortDestinationPathName"));
            }
        }

        public string DestinationPath
        {
            get
            {
                
                return _destinationPath;
            }
            set
            {
                _destinationPath = value;
                string dirName = new DirectoryInfo(DestinationPath).Name;
                ShortDestinationPathName = dirName;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DestinationPath"));
            }
        }

        public RelayCommand CloseWindowCommand { get; set; }
        public RelayCommand ListBoxSelectionChangedCommand { get; set; }
        public RelayCommand SelectDestinationPathCommand { get; set; }
        public RelayCommand CopySelectedFilesCommand { get; set; }
        public RelayCommand OpenPopupCommand { get; set; }

        public MainWindowVM()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == true)
            {
                return;
            }
            SettingsManager configManager = new SettingsManager();
            DestinationPath = configManager.GetDefaultDestinationPath();
            CloseWindowCommand = new RelayCommand(CloseWindow);
            ListBoxSelectionChangedCommand = new RelayCommand(ListboxSelectionChanged);
            SelectDestinationPathCommand = new RelayCommand(SelectDestinationPath);
            CopySelectedFilesCommand = new RelayCommand(CopySelectedFiles);
            OpenPopupCommand = new RelayCommand(OpenPopup);

            tempPath = BuildTempFolderPath();
            sourcePath = BuildImagesSourcePath();
            
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            SynchronizeTempFilesWithSourceFiles();
            CopyNewFoundFiles(sourcePath);
            LoadImagesFromTemp(tempPath);
            
            ImagesView = CollectionViewSource.GetDefaultView(Images);
            ImagesView.SortDescriptions.Add(new SortDescription("CurrentImage.Width", ListSortDirection.Descending));
            ImagesView.GroupDescriptions.Add(new PropertyGroupDescription("Type"));

        }

        private void OpenPopup(object parameter)
        {
            ImageElement image = (ImageElement)parameter;
            ZoomPopup popup = new ZoomPopup();
            popup.SelectedImage = image;
            popup.Title = image.Name;
            if (popup.ShowDialog() == true)
            {

            }
        }

        private void CopySelectedFiles(object parameter)
        {
            if (SelectedImages.Count > 0) 
            { 
                foreach(ImageElement image in SelectedImages)
                {
                    Uri imagePath = image.CurrentImage.UriSource;
                    if (imagePath.IsFile)
                    {
                        //string fileName = System.IO.Path.GetFileName(imagePath.LocalPath);
                        string fileName = Path.ChangeExtension(image.Name, ".jpg");
                        string Destination = Path.Combine(DestinationPath, fileName);
                        if (!File.Exists(Destination))
                        {
                            File.Copy(image.CurrentImage.UriSource.LocalPath, Destination);
                        }
                    }
                }
                ((ListBox)parameter).SelectedItems.Clear();
            }

        }

        private void SelectDestinationPath(object parameter)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                DestinationPath = dialog.SelectedPath;
                
            }
       
        }

        private void ListboxSelectionChanged(object parameter)
        {
            SelectedImages.Clear();
            var images = ((ListBox)parameter).SelectedItems;
            for (int i = 0; i < images.Count; i++)
            {
                SelectedImages.Add((ImageElement)images[i]);
            }
        }

        public string BuildImagesSourcePath()
        {
            string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string combineWith = @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            return Path.Combine(AppDataPath, combineWith);
        }

        public string BuildTempFolderPath()
        {
            string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string combineWith = @"BingDesktopFinder\Temp";
            return Path.Combine(DocumentsPath, combineWith);
        }

        public void CopyNewFoundFiles(string fullPath)
        {
            ObservableCollection<string> files = new ObservableCollection<string>(Directory.GetFiles(fullPath));
            for (int i = 0; i < files.Count; i++)
            {
                FileInfo infos = new FileInfo(files[i]);
                if (infos.Length > 300000)
                {
                    var fileName = Path.GetFileName(files[i]);
                    var newPath = Path.Combine(tempPath, fileName);
                    var newFile = Path.ChangeExtension(newPath, ".jpg");
                    if (!File.Exists(newFile))
                    {
                        File.Copy(files[i], newFile);
                    }
                }
            }
        }

        public void LoadImagesFromTemp(string tempPath)
        {
            foreach (string data in Directory.GetFiles(tempPath))
            {
                BitmapImage background = new BitmapImage(new Uri(data, UriKind.Absolute));
                ImageElement newImage = new ImageElement();
                newImage.CurrentImage = background;
                newImage.Name = Path.GetFileNameWithoutExtension(new FileInfo(data).Name);
                newImage.CreationDate = new FileInfo(data).CreationTimeUtc;
                Images.Add(newImage);
            }
        }

        public void SynchronizeTempFilesWithSourceFiles()
        {
            HashSet<string> SourceElements = new HashSet<string>();
            List<string> tempFiles = new List<string>(Directory.GetFiles(tempPath));
            foreach(string element in Directory.GetFiles(sourcePath))
            {
                SourceElements.Add(Path.GetFileName(element));
            }
            foreach (string data in tempFiles)
            {
                if (!SourceElements.Contains(Path.GetFileNameWithoutExtension(data)))
                {
                    File.Delete(data);
                    Debug.WriteLine(data);
                }
            }
        }

        private void CloseWindow(object parameter)
        {
            Directory.Delete(tempPath);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
