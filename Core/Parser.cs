using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Text.RegularExpressions;

namespace Core
{
    public static class Parser
    {
        /// <summary>
        /// Split a large script into executable units for Roslyn
        /// </summary>
        public static string[] SplitScripts(string text)
        {
            string[] lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            List<string> scripts = new List<string>();

            StringBuilder scriptBuilder = new ();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (RoslynContext.ImportModuleRegex().IsMatch(line)
                    || RoslynContext.IncludeScriptRegex().IsMatch(line)
                    || RoslynContext.HelpItemRegex().IsMatch(line))
                {
                    if (scriptBuilder.Length != 0)
                    {
                        scripts.Add(scriptBuilder.ToString());
                        scriptBuilder.Clear();
                    }
                    scripts.Add(line);
                }
                else
                    scriptBuilder.AppendLine(line);
            }
            if (scriptBuilder.Length > 0)
                scripts.Add(scriptBuilder.ToString().TrimEnd());

            return scripts.ToArray();
        }
    }
}
