using System;
using System.Windows;
using System.Windows.Data;

namespace Notebook.Converters
{
    class CellTypeToBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((CellType)value)
            {
                case CellType.Markdown:
                    return new Thickness(1);
                case CellType.Code:
                    return new Thickness(2);
                case CellType.CacheOutput:
                    return new Thickness(0);
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var thickness = (Thickness)value;
            if (thickness == new Thickness(1))
                return CellType.Markdown;
            else if (thickness == new Thickness(2))
                return CellType.Code;
            else if (thickness == new Thickness(0))
                return CellType.CacheOutput;
            else
                throw new ArgumentException();
        }
    }
}
