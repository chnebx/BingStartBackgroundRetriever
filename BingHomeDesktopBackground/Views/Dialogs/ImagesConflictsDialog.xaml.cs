using BingHomeDesktopBackground.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BingHomeDesktopBackground.Views.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour ImagesConflicts.xaml
    /// </summary>
    public partial class ImagesConflictsDialog : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Dictionary<ImageElement, ImageElement>> _images;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ImageElement> Replaced = new List<ImageElement>();

        public List<ImageElement> Renamed = new List<ImageElement>();

        public ObservableCollection<Dictionary<ImageElement, ImageElement>> Images
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

        public ImagesConflictsDialog(List<Dictionary<ImageElement, ImageElement>> conflicts)
        {
            InitializeComponent();
            this.DataContext = this;
            Images = new ObservableCollection<Dictionary<ImageElement, ImageElement>>(conflicts);
        }

        private void IgnoreBtn_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<ImageElement, ImageElement> item = (Dictionary<ImageElement, ImageElement>)(((Button)sender).DataContext);
            Images.Remove(item);
            CheckIfTotallySolved();
        }

        private void ReplaceBtn_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<ImageElement, ImageElement> item = (Dictionary<ImageElement, ImageElement>)(((Button)sender).DataContext);
            foreach(KeyValuePair<ImageElement, ImageElement> data in item)
            {
                Replaced.Add(data.Key);
                Images.Remove(item);
            }
            CheckIfTotallySolved();
        }

        private void CheckIfTotallySolved()
        {
            if (Images.Count <= 0)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void RenameBtn_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<ImageElement, ImageElement> item = (Dictionary<ImageElement, ImageElement>)(((Button)sender).DataContext);
            foreach (KeyValuePair<ImageElement, ImageElement> data in item)
            {
                RenamePictureDialog renameDialog = new RenamePictureDialog(data.Key.Name);
                if (renameDialog.ShowDialog() == true && renameDialog.PictureName != data.Key.Name)
                {
                    data.Key.Name = renameDialog.PictureName;
                    Renamed.Add(data.Key);
                    Images.Remove(item);
                }
            }
            CheckIfTotallySolved();
        }
    }
}
