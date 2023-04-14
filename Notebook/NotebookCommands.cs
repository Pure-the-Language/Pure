using System.Windows.Input;

namespace Notebook
{
    public static class NotebookCommands
    {
        #region Menu Commands
        public static readonly RoutedUICommand CreateCSharpCellCommand =
            new(
                "Create C# Cell",
                "CreateCSharpCell",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F2, ModifierKeys.None, "F2")
                });
        public static readonly RoutedUICommand CreatePythonCellCommand =
            new(
                "Create Python Cell",
                "CreatePythonCell",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F3, ModifierKeys.None, "F3")
                });
        public static readonly RoutedUICommand CreateMarkdownCellCommand =
            new(
                "Create Markdown Cell",
                "CreateMarkdownCell",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F4, ModifierKeys.None, "F4")
                });
        public static readonly RoutedUICommand CreateCSharpCellWithCopyCommand =
            new(
                "Create C# Cell with Copy",
                "CreateCSharpCellWithCopy",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F2, ModifierKeys.Shift, "Shift+F2")
                });
        public static readonly RoutedUICommand CreatePythonCellWithCopyCommand =
            new(
                "Create Python Cell with Copy",
                "CreatePythonCellWithCopy",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F3, ModifierKeys.Shift, "Shift+F3")
                });
        public static readonly RoutedUICommand CreateMarkdownCellWithCopyCommand =
            new(
                "Create Markdown Cell with Copy",
                "CreateMarkdownCellWithCopy",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F4, ModifierKeys.Shift, "Shift+F4")
                });
        public static readonly RoutedUICommand ExecuteCellCommand =
            new(
                "Execute Cell",
                "ExecuteCell",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F5, ModifierKeys.None, "F5")
                });
        #endregion
    }
}
