using System;
using System.Windows.Data;

namespace Notebook.Converters
{
    class CellTypeToAvalonLanguageTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((CellType)value)
            {
                case CellType.Markdown:
                    return ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("Markdown");
                case CellType.CSharp:
                    return ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#");
                case CellType.Python:
                    return ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("Python");
                case CellType.CacheOutput:
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
