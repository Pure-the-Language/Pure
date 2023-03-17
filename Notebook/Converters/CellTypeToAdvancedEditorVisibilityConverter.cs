using System;
using System.Windows;
using System.Windows.Data;

namespace Notebook.Converters
{
    class CellTypeToAdvancedEditorVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((CellType)value)
            {
                case CellType.Markdown:
                    return Visibility.Collapsed;
                case CellType.Code:
                    return Visibility.Visible;
                case CellType.CacheOutput:
                    return Visibility.Collapsed;
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (Visibility)value;
            if (visibility == Visibility.Collapsed)
                throw new ArgumentException();
            else if (visibility == Visibility.Visible)
                return CellType.Code;
            else
                throw new ArgumentException();
        }
    }
}
