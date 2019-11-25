using System;
using System.Collections.Generic;
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
    /// Logique d'interaction pour RenamePictureDialog.xaml
    /// </summary>
    public partial class RenamePictureDialog : Window
    {
        private string _name;

        public string PictureName
        {
            get { return _name; }
            set { _name = value; }
        }

        public RenamePictureDialog(string name)
        {
            InitializeComponent();
            this.DataContext = this;
            PictureName = name;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NameTxtBox.Text))
            {
                PictureName = NameTxtBox.Text;
                this.DialogResult = true;
            } else
            {
                this.DialogResult = false;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
