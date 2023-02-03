using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class Construct
    {
        #region Main Construct
        public static void Import(string dllName)
        {
            Console.WriteLine("Import");
        }
        public static void Include(string scriptName)
        {
            Console.WriteLine("Script");
        }
        #endregion
    }
}
