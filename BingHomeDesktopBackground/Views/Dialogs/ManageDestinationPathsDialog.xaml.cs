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
        private ObservableCollection<PathElement> _pathsList;

        public ObservableCollection<PathElement> PathsList
        {
            get
            {
                return _pathsList;
            }
            set
            {
                _pathsList = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PathsList"));
            }
        }

        public ManageDestinationPathsDialog(ObservableCollection<PathElement> paths)
        {
            InitializeComponent();
            DataContext = this;
            PathsList = paths;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string selectedPath = FilesManager.OpenFolderDialog();
            PathElement newPath = new PathElement { FullPath = selectedPath };
            PathsList.Add(newPath);
        }
    }
}
