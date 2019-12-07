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
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media;

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
        private bool _selectedEverything;
        private Predicate<object> IsDesktopFilter = new Predicate<object>(x => ((ImageElement)x).Type == ImageType.Desktop);
        private Predicate<object> IsPhoneFilter = new Predicate<object>(x => ((ImageElement)x).Type == ImageType.Phone);
        
        private string _selectedFilterName;
        private bool _isNotLoading = true;

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

        public bool IsNotLoading
        {
            get
            {
                return _isNotLoading;
            }
            set
            {
                _isNotLoading = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsNotLoading"));
            }
        }

        public bool SelectedEverything
        {
            get 
            { 
                return _selectedEverything;
            }
            set 
            { 
                _selectedEverything = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SelectedEverything"));
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
                SettingsManager.UpdateSettingsDefaultDestinationPath(_destinationPath);
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
        public RelayCommand AddDestinationPathCommand { get; set; }
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

            SelectedFilterName = SettingsManager.settings.DefaultFilter;
            ListBoxSelectionChangedCommand = new RelayCommand(ListboxSelectionChanged);
            AddDestinationPathCommand = new RelayCommand(AddDestinationPath);
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
            DestinationPath = SettingsManager.settings.DefaultDestinationPath;

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

        private async void RefreshWallpapers(object parameter)
        {
            IsNotLoading = false;
            List<ImageElement> newImages = await Refresh();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Images.Clear();
                   
                foreach (ImageElement e in newImages)
                {
                    Images.Add(e);
                }
                if (ImagesView == null)
                {
                    CreateView();
                        
                }
                else
                {
                    ImagesView.Refresh();
                }
            });
            IsNotLoading = true;
        }

        private void DefineImagesViewFilter(Predicate<object> predicate)
        {
            if (ImagesView != null)
            {
                ImagesView.Filter = predicate;
                ImagesView.Refresh();
            }
            
        }

        private void OpenSavedPaths(object parameter)
        {
            ManageDestinationPathsDialog savedPathsDialog = new ManageDestinationPathsDialog((ObservableCollection<PathElement>)parameter);
            if (savedPathsDialog.ShowDialog() == true)
            {
                SavePaths(savedPathsDialog.Paths);
                Paths = savedPathsDialog.Paths;
                if (Paths.Count > 0)
                {
                    DestinationPath = Paths[Paths.Count - 1].FullPath;
                } else
                {
                    DestinationPath = null;
                }
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
            SelectedEverything = false;
        }

        private void SelectAll(object parameter)
        {
            ListBox imagesListBox = (ListBox)parameter;
            foreach(ImageElement image in Images)
            {
                imagesListBox.SelectedItems.Add(image);
            }
            SelectedEverything = true;
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
            List<Dictionary<ImageElement, ImageElement>> conflicts = new List<Dictionary<ImageElement, ImageElement>>();
            try
            {
                if (SelectedImages.Count > 0)
                {
                    bool success = false;
                    List<ImageElement> conflictsFound = Copy(SelectedImages, out success);
                    while (conflictsFound.Count > 0)
                    {
                        conflictsFound = Copy(new ObservableCollection<ImageElement>(conflictsFound), out success);
                    }
                ((ListBox)parameter).SelectedItems.Clear();
                    if (SelectedEverything)
                    {
                        SelectedEverything = false;
                    }
                }
            } catch(Exception e)
            {
                MessageBox.Show("There was an error, you might want to check the destination path and make sure the one provided is valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

        }

        private List<ImageElement> Copy(ObservableCollection<ImageElement> images, out bool successful)
        {
            successful = true;
            List<Dictionary<ImageElement, ImageElement>> conflicts = new List<Dictionary<ImageElement, ImageElement>>();
            foreach (ImageElement image in images)
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
                    else if (image.CanReplace)
                    {
                        File.Copy(image.CurrentImage.UriSource.LocalPath, Destination, true);
                        image.CanReplace = false;
                    }
                    else
                    {
                        successful = false;
                        Dictionary<ImageElement, ImageElement> result = new Dictionary<ImageElement, ImageElement>();
                        ImageElement conflictingPicture = CreateImageFromFile(Destination);
                        result.Add(image, conflictingPicture);
                        conflicts.Add(result);
                    }
                }
            }
            if (conflicts.Count > 0)
            {
                ImagesConflictsDialog conflictsDialog = new ImagesConflictsDialog(conflicts);
                if (conflictsDialog.ShowDialog() == true)
                {
                    return conflictsDialog.Fixed;
                }
            }
            return new List<ImageElement>();
        }

        private void AddDestinationPath(object parameter)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                Paths.Add(new PathElement() { FullPath = dialog.SelectedPath });
                DestinationPath = dialog.SelectedPath;
                SettingsManager.UpdateSettingsDestinationPaths(Paths);
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

        public async Task<List<ImageElement>> Refresh()
        {
            List<ImageElement> LoadedImages = new List<ImageElement>();
            await SynchronizeTempFilesWithSourceFiles();
            try
            {
                Application.Current.Dispatcher.Invoke(() => {
                    LoadedImages = LoadImagesFromTemp(SettingsManager.settings.DefaultTempPath);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show("The source folder isn't set correctly, please go to settings and change it", "Source Path Has Incompatible Files", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadedImages = new List<ImageElement>();
            }
            return LoadedImages;
        }

        public ImageElement CreateImageFromFile(string path)
        {
            BitmapImage background = new BitmapImage();
            background.BeginInit();
            background.CacheOption = BitmapCacheOption.OnLoad;
            background.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            background.UriSource = new Uri(path, UriKind.Absolute);
            background.EndInit();
            ImageSource TemplateImage = background;
            ImageElement newImage = new ImageElement();
            newImage.Image = TemplateImage;
            newImage.CurrentImage = background;
            newImage.Name = Path.GetFileNameWithoutExtension(new FileInfo(path).Name);
            newImage.CreationDate = new FileInfo(path).CreationTimeUtc;
            return newImage;
        }

        public List<ImageElement> LoadImagesFromTemp(string tempPath)
        {
            List<ImageElement> images = new List<ImageElement>();
            foreach (string data in Directory.GetFiles(tempPath))
            {
                images.Add(CreateImageFromFile(data));
            }
            return images;
        }

        public bool CheckFileIsWallpaper(string path)
        {
            try
            {
                Bitmap img = new Bitmap(path);
                if (img.Width > 1000 && img.Height > 1000)
                {
                    img.Dispose();
                    return true;
                }
                img.Dispose();
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task SynchronizeTempFilesWithSourceFiles()
        {
            await Task.Run(() => {
                HashSet<string> sourceElements = new HashSet<string>();
                HashSet<string> tempElements = new HashSet<string>();
                List<string> tempFiles = new List<string>(Directory.GetFiles(SettingsManager.settings.DefaultTempPath));
                foreach (string tempElement in Directory.GetFiles(SettingsManager.settings.DefaultTempPath))
                {
                    tempElements.Add(Path.GetFileNameWithoutExtension(tempElement));
                }
                foreach (string sourceElement in Directory.GetFiles(SettingsManager.settings.DefaultSourcePath))
                {
                    if (CheckFileIsWallpaper(sourceElement))
                    {
                        string fileName = Path.GetFileName(sourceElement);
                        sourceElements.Add(fileName);
                        if (!tempElements.Contains(fileName))
                        {
                            var newPath = Path.Combine(SettingsManager.settings.DefaultTempPath, fileName);
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
            });

          
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
