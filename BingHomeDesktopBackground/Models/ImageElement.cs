using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media.Imaging;

namespace BingHomeDesktopBackground.Models
{
    public class ImageElement: INotifyPropertyChanged
    {
        private BitmapImage _currentImage;
        private bool _isSelected;
        private DateTime _creationDate;

        public enum ImageType
        {
            Desktop,
            Phone
        }

        public ImageElement()
        {
            Type = ImageType.Desktop;
        }

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

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
