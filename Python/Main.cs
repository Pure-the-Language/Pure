using Python.Runtime;

namespace Python
{
    public static class Main
    {
        #region Initialization
        private static PyModule Scope;
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
                    Scope = Py.CreateScope();
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
                Scope.Dispose();
            }
            PythonEngine.Shutdown();
        }
        #endregion

        public static dynamic Evaluate(string snippet)
        {
            if (Scope == null) 
            {
                Console.WriteLine("Python runtime is not initialized.");
                return null;
            }
            using (Py.GIL())
            {
                dynamic result = Scope.Eval(snippet);
                if (result != null && result.ToString() != "None")
                {
                    Console.WriteLine(result.ToString());
                    return result;
                }
                else return null;
            }
        }
    }
}