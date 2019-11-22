using BingHomeDesktopBackground.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace BingHomeDesktopBackground.Converters
{
    public class FilterNameToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == parameter.ToString())
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() != parameter.ToString())
                return false;
            return true;
        }
    }
}
