﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace BingHomeDesktopBackground.Models
{
    [Table("Settings")]
    public class Settings: INotifyPropertyChanged
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string DefaultDestinationPath { get; set; }
        public string DefaultSourcePath { get; set; }
        public string DefaultTempPath { get; set; }
        public string DefaultFilter { get; set; }

        [TextBlob("Destinations")]
        public List<string> DestinationPaths { get; set; }
        public string Destinations { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
