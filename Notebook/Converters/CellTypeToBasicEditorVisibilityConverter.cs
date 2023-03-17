using System;
using System.Windows;
using System.Windows.Data;

namespace Notebook.Converters
{
    class CellTypeToBasicEditorVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((CellType)value)
            {
                case CellType.Markdown:
                    return Visibility.Visible;
                case CellType.Code:
                    return Visibility.Collapsed;
                case CellType.CacheOutput:
                    return Visibility.Visible;
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (Visibility)value;
            if (visibility == Visibility.Collapsed)
                return CellType.Code; 
            else if (visibility == Visibility.Visible)
                throw new ArgumentException();
            else
                throw new ArgumentException();
        }
    }
}
