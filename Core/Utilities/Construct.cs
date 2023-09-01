namespace Core.Utilities
{
    /// <summary>
    /// Provides some global-level functions that's exposed at the root of Roslyn context;
    /// Refer to <see cref="RoslynContext(bool, Action{string}, string)"/>
    /// </summary>
    public static class Construct
    {
        #region Helper Construct
        public static void Help(object instance)
        {
            var type = instance.GetType();
            RoslynContext.PrintType(type);
        }
        #endregion
    }
}
