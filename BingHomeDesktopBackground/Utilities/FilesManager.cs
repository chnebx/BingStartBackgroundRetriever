using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingHomeDesktopBackground.Utilities
{
    public class FilesManager
    {
        public FilesManager()
        {

        }

        public static string OpenFolderDialog(string atPath=null)
        {
           
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if (atPath != null)
            {
                atPath = $"{atPath}\\";
                dialog.SelectedPath = atPath;
            }
            if (dialog.ShowDialog() == true)
            {
                return dialog.SelectedPath;
            }
            return null;
        }
    }
}
