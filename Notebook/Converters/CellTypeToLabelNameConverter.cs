using System;
using System.Windows.Data;

namespace Notebook.Converters
{
    class CellTypeToLabelNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((CellType)value)
            {
                case CellType.Markdown:
                    return "Markdown";
                case CellType.Code:
                    return "Code";
                case CellType.CacheOutput:
                    return "Cached Output";
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((string)value)
            {
                case "Code":
                    return CellType.Code;
                case "Markdown":
                    return CellType.Markdown;
                case "Cached Output":
                    return CellType.CacheOutput;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
