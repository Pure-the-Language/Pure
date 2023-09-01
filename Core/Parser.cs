using System.Text;
using System.Text.RegularExpressions;

namespace Core
{
    public static class Parser
    {
        #region Versioning
        public static readonly string CoreVersion = "v0.1.0";
        public static readonly string VersionChangelog = """
            * v0.0.1-v0.0.3: Misc. basic functional implementations.
            * v0.0.3: Add functional Nuget implementation; Add support for `nugetRepoIdentifier:string` parameter for Interpreter and Roslyn Context.
            * v0.0.4: Change exception handling logic, print stack trace for easier debugging.
            * v0.0.5: Update runtime exception handling behavior.
            * v0.0.5.1: Fix issue with literal numerical array parsing.
            * v0.1.0: Fix semantics of `Include()`; Bump minor version number.
            """;
        #endregion

        #region Main Function
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
            StringBuilder scriptBuilder = new();
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
        #endregion
    }
}
