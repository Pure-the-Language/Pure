using System.Windows.Input;

namespace Notebook
{
    public static class NotebookCommands
    {
        #region Menu Commands
        public static readonly RoutedUICommand CreateCellCommand =
            new(
                "Create Cell",
                "CreateCell",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F2, ModifierKeys.None, "F2")
                });
        public static readonly RoutedUICommand DeleteCellCommand =
            new(
                "Delete Cell",
                "DeleteCell",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F3, ModifierKeys.None, "F3")
                });
        public static readonly RoutedUICommand ExecuteCellCommand =
            new(
                "Execute Cell",
                "ExecuteCell",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F5, ModifierKeys.None, "F5")
                });
        public static readonly RoutedUICommand ExecuteAllCellsCommand =
            new(
                "Execute All Cells",
                "ExecuteAllCells",
                typeof(NotebookCommands),
                new InputGestureCollection {
                    new KeyGesture(Key.F6, ModifierKeys.None, "F6")
                });
        #endregion
    }
}
