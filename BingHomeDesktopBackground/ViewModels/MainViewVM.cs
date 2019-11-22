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
using BingHomeDesktopBackground.Views.Dialogs;
using static BingHomeDesktopBackground.Models.ImageElement;

namespace BingHomeDesktopBackground.ViewModels
{
    public class MainViewVM: INotifyPropertyChanged
    {
        private ObservableCollection<ImageElement> _images = new ObservableCollection<ImageElement>();
        private ObservableCollection<ImageElement> _selectedImages = new ObservableCollection<ImageElement>();
        private ObservableCollection<string> _results;
        private string tempPath;
        private string _shortDestinationPathName;
        private ICollectionView _imagesView;
        private string _destinationPath;
        private ObservableCollection<PathElement> _paths;
        private Predicate<object> IsDesktopFilter = new Predicate<object>(x => ((ImageElement)x).Type == ImageType.Desktop);
        private Predicate<object> IsPhoneFilter = new Predicate<object>(x => ((ImageElement)x).Type == ImageType.Phone);
        private string _selectedFilterName;

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

        public ObservableCollection<PathElement> Paths
        {
            get
            {
                return _paths;
            }
            set
            {
                _paths = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Paths"));
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
                if (value != null)
                {
                    string dirName = new DirectoryInfo(DestinationPath).Name;
                    ShortDestinationPathName = dirName;
                }
                
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DestinationPath"));
            }
        }

        public string SelectedFilterName
        {
            get
            {
                return _selectedFilterName;
            }
            set
            {
                _selectedFilterName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SelectedFilterName"));
            }
        }

        public RelayCommand ListBoxSelectionChangedCommand { get; set; }
        public RelayCommand SelectDestinationPathCommand { get; set; }
        public RelayCommand CopySelectedFilesCommand { get; set; }
        public RelayCommand SelectAllCommand { get; set; }
        public RelayCommand UnSelectAllCommand { get; set; }
        public RelayCommand OpenPopupCommand { get; set; }
        public RelayCommand OpenSavedPathsCommand { get; set; }
        public RelayCommand RefreshWallpapersCommand { get; set; }
        public RelayCommand SelectFilterCommand { get; set; }

        public MainViewVM()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == true)
            {
                return;
            }
            DestinationPath = SettingsManager.settings.DefaultDestinationPath;
            SelectedFilterName = SettingsManager.settings.DefaultFilter;
            ListBoxSelectionChangedCommand = new RelayCommand(ListboxSelectionChanged);
            SelectDestinationPathCommand = new RelayCommand(SelectDestinationPath);
            CopySelectedFilesCommand = new RelayCommand(CopySelectedFiles);
            SelectAllCommand = new RelayCommand(SelectAll);
            UnSelectAllCommand = new RelayCommand(UnSelectAll);
            OpenPopupCommand = new RelayCommand(OpenPopup);
            OpenSavedPathsCommand = new RelayCommand(OpenSavedPaths);
            RefreshWallpapersCommand = new RelayCommand(RefreshWallpapers);
            SelectFilterCommand = new RelayCommand(SelectFilter);
            Paths = new ObservableCollection<PathElement>();

            tempPath = SettingsManager.settings.DefaultTempPath;
            sourcePath = SettingsManager.settings.DefaultSourcePath;
            List<string> paths = SettingsManager.settings.DestinationPaths;
            foreach(string path in paths)
            {
                Paths.Add(new PathElement() { FullPath=path });
            }

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            RefreshWallpapers(null);
            SelectFilter(SelectedFilterName);
        }

        private void SelectFilter(object parameter)
        {
            if (parameter != null && parameter is string)
            {
                SelectedFilterName = parameter.ToString();
                SettingsManager.settings.DefaultFilter = SelectedFilterName;
                switch (parameter.ToString())
                {
                    case "DesktopFilter":
                        DefineImagesViewFilter(IsDesktopFilter);
                        break;
                    case "PhoneFilter":
                        DefineImagesViewFilter(IsPhoneFilter);
                        break;
                    case "NoFilter":
                    default:
                        DefineImagesViewFilter(null);
                        break;
                }
            }
        }

        private void RefreshWallpapers(object parameter)
        {

            Images.Clear();
            SettingsManager.RefreshWallpapers();
            foreach(ImageElement e in SettingsManager.LoadedImages)
            {
                Images.Add(e);
            }
            if (ImagesView == null)
            {
                CreateView();
            } else
            {
                ImagesView.Refresh();
            }
            
        }

        private void DefineImagesViewFilter(Predicate<object> predicate)
        {
            ImagesView.Filter = predicate;
            ImagesView.Refresh();
        }

        private void OpenSavedPaths(object parameter)
        {
            ManageDestinationPathsDialog savedPathsDialog = new ManageDestinationPathsDialog((ObservableCollection<PathElement>)parameter);
            if (savedPathsDialog.ShowDialog() == true)
            {
                SavePaths(savedPathsDialog.Paths);
                Paths = savedPathsDialog.Paths;
            }
        }

        private void SavePaths(ObservableCollection<PathElement> paths)
        {
            List<string> newPaths = new List<string>();
            foreach (PathElement path in paths)
            {
                newPaths.Add(path.FullPath);
            }
            SettingsManager.SaveDestinationPaths(newPaths);
        }

        private void UnSelectAll(object parameter)
        {
            ListBox imagesListBox = (ListBox)parameter;
            imagesListBox.SelectedItems.Clear();
        }

        private void SelectAll(object parameter)
        {
            ListBox imagesListBox = (ListBox)parameter;
            foreach(ImageElement image in Images)
            {
                imagesListBox.SelectedItems.Add(image);
            }
        }

        public void CreateView()
        {
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
                Paths.Add(new PathElement() { FullPath = DestinationPath });
                SettingsManager.SaveDefaultDestinationPath(DestinationPath);
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
