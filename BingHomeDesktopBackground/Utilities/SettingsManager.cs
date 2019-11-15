using BingHomeDesktopBackground.Models;
using Microsoft.Extensions.Configuration;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

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


        public static void InitData()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseName))
            {
                conn.CreateTable<Settings>();
                settings = conn.GetAllWithChildren<Settings>()[0];
                if (settings == null)
                {
                    settings = new Settings
                    {
                        ID = 0,
                        DefaultDestinationPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                        DefaultTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"BingDesktopFinder\Temp"),
                        DefaultSourcePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets"),
                        DestinationPaths = new List<string>()
                    };
                    conn.Insert(settings);
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

        public static void SaveSettings(string sourcePath, string tempPath)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseName))
            {
                conn.CreateTable<Settings>();
                settings.DefaultTempPath = tempPath;
                settings.DefaultSourcePath = sourcePath;
                conn.Update(settings);
            }
        }
        
        public static ObservableCollection<ImageElement> LoadedImages { get; set; }

        public static string ConnectionString = "Settings.db";


        public string GetDefaultDestinationPath()
        {
            return "";
        }

    }
}
