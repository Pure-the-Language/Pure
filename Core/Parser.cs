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
            string[] lines = text
                .Replace("\r", string.Empty)
                .Split('\n')
                .ToArray();
            List<string> scripts = new List<string>();

            bool currentLineIsInBlockComment = false;
            int totalLineCounter = 0;
            StringBuilder scriptBuilder = new ();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (!currentLineIsInBlockComment &&
                    (RoslynContext.ImportModuleRegex().IsMatch(line)
                    || RoslynContext.IncludeScriptRegex().IsMatch(line)
                    || RoslynContext.HelpItemRegex().IsMatch(line)))
                {
                    if (scriptBuilder.Length != 0)
                    {
                        scripts.Add(new string('\n', totalLineCounter) + scriptBuilder.ToString().Trim());
                        totalLineCounter += string.IsNullOrWhiteSpace(scriptBuilder.ToString()) ? 1 : scriptBuilder.ToString().Split('\n').Length;
                        scriptBuilder.Clear();
                    }
                    scripts.Add(line);
                    totalLineCounter++;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(line))
                        scriptBuilder.Append("\n");
                    else if (scriptBuilder.Length == 0) 
                        scriptBuilder.Append(line);
                    else
                        scriptBuilder.Append("\n" + line);
                }

                // Block comment handling
                if (Regex.Matches(line, @"/\*").Count != Regex.Matches(line, @"\*/").Count) // Remark-cz: This is not robust but it works😆
                    currentLineIsInBlockComment = !currentLineIsInBlockComment;
            }
            if (scriptBuilder.Length > 0)
                scripts.Add(new string('\n', totalLineCounter) + scriptBuilder.ToString().Trim());

            return scripts.ToArray();
        }
    }
}
