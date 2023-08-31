using System;
using System.Windows;
using System.Windows.Data;

namespace Notebook.Converters
{
    class CellTypeToTextWrappingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((CellType)value)
            {
                case CellType.CacheOutput:
                    return TextWrapping.NoWrap;
                case CellType.Markdown:
                case CellType.CSharp:
                case CellType.Python:
                    return TextWrapping.WrapWithOverflow;
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
