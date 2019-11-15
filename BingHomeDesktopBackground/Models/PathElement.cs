using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace BingHomeDesktopBackground.Models
{
    public class PathElement: INotifyPropertyChanged
    {
        private string _fullPath;
        private string _shortPath;

        public PathElement()
        {

        }

        public string FullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                _fullPath = value;
                try
                {
                    _shortPath = new DirectoryInfo(_fullPath).Name;
                } catch(Exception e)
                {
                    _shortPath = "Undefined";
                }
                
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("FullPath"));
            }
        }

        public string ShortPath
        {
            get
            {
                return _shortPath;
            }
            set
            {
                _shortPath = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShortPath"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
