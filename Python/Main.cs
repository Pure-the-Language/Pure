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
                Python.Runtime.Runtime.PythonDLL = "python310.dll";
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

        public static void Evaluate(string snippet)
        {
            if (Scope == null) 
            {
                Console.WriteLine("Python runtime is not initialized.");
                return;
            }
            using (Py.GIL())
            {
                dynamic result = Scope.Eval(snippet);
                if (result != null && result.ToString() != "None")
                    Console.WriteLine(result.ToString());
            }
        }
    }
}