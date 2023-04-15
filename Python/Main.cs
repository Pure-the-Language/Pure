﻿using Python.Runtime;

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
        public static void Evaluate(string snippet)
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