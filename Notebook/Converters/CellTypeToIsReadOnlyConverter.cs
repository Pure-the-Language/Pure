using System;
using System.Windows;
using System.Windows.Data;

namespace Notebook.Converters
{
    class CellTypeToIsReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((CellType)value == CellType.CacheOutput)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return CellType.CacheOutput;
            else 
                throw new ArgumentException();
        }
    }
}
