using BingHomeDesktopBackground.Models;
using Microsoft.Extensions.Configuration;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BingHomeDesktopBackground.Utilities
{
    public class SettingsManager
    {
        public static Settings settings;

        public SettingsManager()
        {
            
        }

        public static string DatabaseName = "Settings.db";

        public static void Init()
        {
            InitData();
        }


        public static string BuildImagesSourcePath()
        {
            string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string combineWith = @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            return Path.Combine(AppDataPath, combineWith);
        }

        public static string BuildTempFolderPath()
        {
            string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string combineWith = @"BingDesktopFinder\Temp";
            return Path.Combine(DocumentsPath, combineWith);
        }


        public static void InitData()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseName))
            {
                conn.CreateTable<Settings>();
                var settingsExist = conn.GetAllWithChildren<Settings>();
                if (settingsExist.Count > 0)
                {
                    settings = settingsExist[0];
                }
                if (settings == null)
                {
                    settings = new Settings
                    {
                        ID = 0,
                        DefaultDestinationPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                        DefaultTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"BingDesktopFinder\Temp"),
                        DefaultSourcePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets"),
                        DestinationPaths = new List<string>(),
                        DefaultFilter = "NoFilter"
                    };
                    settings.DestinationPaths.Add(settings.DefaultDestinationPath);
                    conn.InsertWithChildren(settings);
                } else if (settings.DestinationPaths == null)
                {
                    settings.DestinationPaths = new List<string>();
                }
            }
        }

        public static void SaveDefaultDestinationPath(string path)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseName))
            {
                conn.CreateTable<Settings>();
                settings.DefaultDestinationPath = path;
                if (settings.DestinationPaths == null)
                {
                    settings.DestinationPaths = new List<string>();
                }
                settings.DestinationPaths.Add(path);
                conn.UpdateWithChildren(settings);
            }
        }

        public static void SaveDestinationPaths(List<string> paths)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseName))
            {
                conn.CreateTable<Settings>();
                settings.DestinationPaths = paths;
                conn.UpdateWithChildren(settings);
            }   
        }

        public static void SaveSettingsPaths(string sourcePath, string tempPath)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseName))
            {
                conn.CreateTable<Settings>();
                settings.DefaultTempPath = tempPath;
                settings.DefaultSourcePath = sourcePath;
                conn.Update(settings);
            }
        }

        public static void SaveSettings()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseName))
            {
                conn.CreateTable<Settings>();
                conn.UpdateWithChildren(settings);
            }
        }



        public static void SynchronizeTempFilesWithSourceFiles()
        {
            HashSet<string> sourceElements = new HashSet<string>();
            HashSet<string> tempElements = new HashSet<string>();
            List<string> tempFiles = new List<string>(Directory.GetFiles(settings.DefaultTempPath));
            foreach (string tempElement in Directory.GetFiles(settings.DefaultTempPath))
            {
                tempElements.Add(Path.GetFileNameWithoutExtension(tempElement));
            }
            foreach (string sourceElement in Directory.GetFiles(settings.DefaultSourcePath))
            {
                if (CheckFileIsWallpaper(sourceElement))
                {
                    string fileName = Path.GetFileName(sourceElement);
                    sourceElements.Add(fileName);
                    if (!tempElements.Contains(fileName))
                    {
                        var newPath = Path.Combine(settings.DefaultTempPath, fileName);
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

        public static bool CheckFileIsWallpaper(string path)
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

        public static ObservableCollection<ImageElement> LoadImagesFromTemp(string tempPath)
        {
            ObservableCollection<ImageElement> images = new ObservableCollection<ImageElement>();
            foreach (string data in Directory.GetFiles(tempPath))
            {
                BitmapImage background = new BitmapImage();
                background.BeginInit();
                background.CacheOption = BitmapCacheOption.OnLoad;
                background.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                background.UriSource = new Uri(data, UriKind.Absolute);
                background.EndInit();
                ImageSource TemplateImage = background;
                ImageElement newImage = new ImageElement();
                newImage.Image = TemplateImage;
                newImage.CurrentImage = background;
                newImage.Name = Path.GetFileNameWithoutExtension(new FileInfo(data).Name);
                newImage.CreationDate = new FileInfo(data).CreationTimeUtc;
                images.Add(newImage);
            }
            return images;
        }

        public static void RefreshWallpapers()
        {
            LoadedImages = new ObservableCollection<ImageElement>();
            SynchronizeTempFilesWithSourceFiles();
            LoadedImages = LoadImagesFromTemp(settings.DefaultTempPath);
        }

        public static ObservableCollection<ImageElement> LoadedImages { get; set; } = new ObservableCollection<ImageElement>();

        public static string ConnectionString = "Settings.db";


        public string GetDefaultDestinationPath()
        {
            return "";
        }

    }
}
