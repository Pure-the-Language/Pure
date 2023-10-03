using System.Reflection;

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
            if (instance is MethodInfo method)
                Console.WriteLine(RoslynContext.PrintMethod(method));
            else if (instance is Delegate @delegate)
                Console.WriteLine(RoslynContext.PrintMethod(@delegate.Method));
            else
            {
                var type = instance.GetType();
                RoslynContext.PrintType(type);
            }
        }
        #endregion

        #region Runtime Evaluation
        internal static Interpreter CurrentInterpreter;
        public static void Parse(string script)
        {
            if (CurrentInterpreter != null)
                CurrentInterpreter.Parse(script);
            else throw new ApplicationException("Interpreter is not initialized.");
        }
        #endregion
    }
}
