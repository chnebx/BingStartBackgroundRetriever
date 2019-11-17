using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BingHomeDesktopBackground.Models
{
    public class ImageElement: INotifyPropertyChanged
    {
        private BitmapImage _currentImage;
        private DateTime _creationDate;
        private string _name;

        public enum ImageType
        {
            Desktop,
            Phone
        }

        public ImageElement()
        {
            Type = ImageType.Desktop;
        }

        public ImageSource Image { get; set; }

        public BitmapImage CurrentImage
        {
            get
            {
                return _currentImage;
            }
            set
            {
                _currentImage = value;
                if (_currentImage != null && _currentImage.Width < _currentImage.Height)
                {
                    Type = ImageType.Phone;
                }
            }
        }

        public ImageType Type { get; set; }
        
        public DateTime CreationDate
        {
            get
            {
                return _creationDate;
            }
            set
            {
                _creationDate = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name")); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
