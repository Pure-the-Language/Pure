using System.Reflection;

namespace Core.Services
{
    public static class AssemblyHelper
    {
        public static string ExecutingAssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(codeBase);
            }
        }
    }
}
