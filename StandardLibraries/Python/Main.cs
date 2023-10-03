using Python.Runtime;

namespace Python
{
    public static class Main
    {
        #region Initialization
        private static PyModule PythonScope;
        private static void StartUp()
        {
            try
            {
                var installedPython = RuntimeHelper.FindPythonDLL();
                if (installedPython == null)
                    throw new ArgumentException("Cannot find any usable Python installation on the machine.");

                Runtime.Runtime.PythonDLL = installedPython;
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
                using (Py.GIL())
                {
                    PythonScope = Py.CreateScope();
                }
                Parse("""
                    import clr
                    clr.AddReference("Python")
                    import Python
                    def print(o):
                    	Python.Main.Print(o)
                    """);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                PythonEngine.Shutdown();
            }
        }
        private static void ShutDown()
        {
            using (Py.GIL())
            {
                PythonScope.Dispose();
            }
            PythonEngine.Shutdown();
        }
        #endregion

        #region Print
        public static void Print(object o)
        {
            Console.WriteLine(o.ToString());
        }
        #endregion

        #region Method
        public static void Parse(string snippet)
        {
            if (PythonScope == null)
            {
                Console.WriteLine("Python runtime is not initialized.");
            }
            using (Py.GIL())
            {
                PythonScope.Exec(snippet);
            }
        }
        public static object ReadPythonScope(Func<dynamic, object> func)
        {
            using (Py.GIL())
            {
                return func(PythonScope);
            }
        }
        public static void ModifyPythonScope(Action<dynamic> func)
        {
            using (Py.GIL())
            {
                func(PythonScope);
            }
        }
        #endregion
    }
}