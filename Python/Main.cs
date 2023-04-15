using Python.Runtime;

namespace Python
{
    public static class Main
    {
        #region Initialization
        private static void StartUp()
        {
            try
            {
                var installedPython = RuntimeHelper.FindPythonDLL();
                if (installedPython == null)
                    throw new ArgumentException("Cannot find any usable Python installation on the machine.");

                Python.Runtime.Runtime.PythonDLL = installedPython;
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
                using (Py.GIL())
                {
                    PythonScope = Py.CreateScope();
                }
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

        #region Method
        public static dynamic Evaluate(string snippet)
        {
            if (PythonScope == null)
            {
                Console.WriteLine("Python runtime is not initialized.");
                return null;
            }
            using (Py.GIL())
            {
                PythonScope.Exec(snippet);
                return PythonScope;
            }
        }
        public static dynamic PythonScope { get; private set; }
        #endregion
    }
}