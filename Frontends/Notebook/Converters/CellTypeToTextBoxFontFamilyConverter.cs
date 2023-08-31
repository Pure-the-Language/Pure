using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Notebook.Converters
{
    class CellTypeToTextBoxFontFamilyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((CellType)value)
            {
                case CellType.CacheOutput:
                    return new FontFamily("Consolas");
                case CellType.CSharp:
                case CellType.Python:
                case CellType.Markdown:
                    return null;
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
