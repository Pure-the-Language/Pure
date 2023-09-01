namespace Core.Utilities
{
    /// <summary>
    /// Provides some global-level functions that's exposed at the root of Roslyn context;
    /// Refer to <see cref="RoslynContext(bool, Action{string}, string)"/>
    /// </summary>
    public static class Construct
    {
        #region Type Help
        public static void Help(object instance)
        {
            var type = instance.GetType();
            RoslynContext.PrintType(type);
        }
        #endregion

        #region Runtime Evaluation
        internal static Interpreter CurrentInterpreter;
        public static void Evaluate(string script)
        {
            if (CurrentInterpreter != null)
                CurrentInterpreter.Evaluate(script);
            else throw new ApplicationException("Interpreter is not initialized.");
        }
        #endregion
    }
}
