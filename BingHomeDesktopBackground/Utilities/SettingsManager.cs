using BingHomeDesktopBackground.Models;
using Microsoft.Extensions.Configuration;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
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

        public static void InitData()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseName))
            {
                conn.CreateTable<Settings>();
                settings = conn.Table<Settings>().FirstOrDefault();
                if (settings == null)
                {
                    settings = new Settings
                    {
                        DefaultDestinationPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                        DefaultTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BingDesktopFinder")
                    };
                    conn.Insert(settings);
                }
            }
        }

       

        public static string ConnectionString = "Settings.db";

        public string GetDefaultDestinationPath()
        {
            return "";
        }

    }
}
