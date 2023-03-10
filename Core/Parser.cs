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
                .Select(line => Regex.Replace(line, @"//.*$", string.Empty))    // Deal with line comments
                .ToArray();
            List<string> scripts = new List<string>();

            bool inCodeBlock = false;
            bool inFluentAPI = false;
            StringBuilder codeBuilder = new ();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (RoslynContext.ImportModuleRegex().IsMatch(line)
                    || RoslynContext.IncludeScriptRegex().IsMatch(line)
                    || RoslynContext.HelpItemRegex().IsMatch(line))
                    scripts.Add(line);
                else if (RoslynContext.LineAssignmentRegex().IsMatch(line))
                {
                    if (i != lines.Length - 1 && !lines[i + 1].TrimStart().StartsWith('.'))
                        scripts.Add(line);
                    else
                    {
                        codeBuilder.AppendLine(line);
                        inFluentAPI = true;
                    }
                }
                else if (inCodeBlock)
                    codeBuilder.AppendLine(line);
                else if (line.Trim() == "}")
                {
                    inCodeBlock = false;
                    scripts.Add(codeBuilder.ToString().TrimEnd());
                    codeBuilder.Clear();
                }
                else if (line.Trim().EndsWith(';'))
                {
                    inCodeBlock = false;
                    inFluentAPI = false;
                    codeBuilder.AppendLine(line);
                    scripts.Add(codeBuilder.ToString().TrimEnd());
                    codeBuilder.Clear();
                }
                else if (inFluentAPI && i != lines.Length - 1 && !lines[i + 1].TrimStart().StartsWith('.'))
                {
                    inFluentAPI = false;
                    codeBuilder.AppendLine(line);
                    scripts.Add(codeBuilder.ToString().TrimEnd());
                    codeBuilder.Clear();
                }
                else
                {
                    codeBuilder.AppendLine(line);
                    if (!inFluentAPI)
                        inCodeBlock = true;
                }
            }
            if (codeBuilder.Length > 0)
                scripts.Add(codeBuilder.ToString().TrimEnd());

            return scripts.ToArray();
        }
    }
}
