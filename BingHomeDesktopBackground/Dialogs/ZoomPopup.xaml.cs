using BingHomeDesktopBackground.Models;
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

namespace BingHomeDesktopBackground.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour ZoomPopup.xaml
    /// </summary>
    public partial class ZoomPopup : Window
    {
        public ZoomPopup()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ImageElement SelectedImage
        {
            get { return (ImageElement)GetValue(SelectedImageProperty); }
            set { SetValue(SelectedImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedImageProperty =
            DependencyProperty.Register("SelectedImage", typeof(ImageElement), typeof(ZoomPopup), new PropertyMetadata(new ImageElement()));


    }
}
