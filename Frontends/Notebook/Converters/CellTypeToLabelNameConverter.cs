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
                case CellType.CSharp:
                    return "C#";
                case CellType.Python:
                    return "Python";
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
                case "C#":
                    return CellType.CSharp;
                case "Python":
                    return CellType.Python;
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
