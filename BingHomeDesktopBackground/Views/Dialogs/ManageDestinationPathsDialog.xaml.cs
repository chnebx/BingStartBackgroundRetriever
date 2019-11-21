using BingHomeDesktopBackground.Models;
using BingHomeDesktopBackground.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Logique d'interaction pour ManageDestinationPathsDialog.xaml
    /// </summary>
    public partial class ManageDestinationPathsDialog : Window, INotifyPropertyChanged
    {
        private PathElement _selectedPath;
        private ObservableCollection<PathElement> _paths;

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

        public PathElement SelectedPath
        {
            get
            {
                return _selectedPath;
            }
            set
            {
                _selectedPath = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SelectedPath"));
            }
        }

        private void CreateBackup(ObservableCollection<PathElement> paths)
        {
            Paths = new ObservableCollection<PathElement>();
            foreach(PathElement path in paths)
            {
                Paths.Add(path); 
            }
        }

        public ManageDestinationPathsDialog(ObservableCollection<PathElement> paths)
        {
            InitializeComponent();
            DataContext = this;
            CreateBackup(paths);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string selectedPath = FilesManager.OpenFolderDialog();
            PathElement newPath = new PathElement { FullPath = selectedPath };
            Paths.Add(newPath);
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Paths.Remove(SelectedPath);
            SelectedPath = null;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
