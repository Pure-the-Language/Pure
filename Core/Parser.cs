using System.Text;

namespace Core
{
    public static class Parser
    {
        /// <summary>
        /// Split a large script into executable units for Roslyn
        /// </summary>
        public static string[] SplitScripts(string text)
        {
            string[] lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> scripts = new List<string>();

            bool inCodeBlock = false;
            StringBuilder codeBuilder = new StringBuilder();
            foreach (var line in lines)
            {
                if (RoslynContext.ImportModuleRegex().IsMatch(line)
                    || RoslynContext.IncludeScriptRegex().IsMatch(line)
                    || RoslynContext.HelpItemRegex().IsMatch(line))
                    scripts.Add(line);
                else if (inCodeBlock)
                    codeBuilder.AppendLine(line);
                else if (line.Trim() == "}")
                {
                    inCodeBlock = false;
                    scripts.Add(codeBuilder.ToString());
                    codeBuilder.Clear();
                }
                else if (line.Trim().EndsWith(';'))
                    scripts.Add(line);
                else
                {
                    codeBuilder.AppendLine(line);
                    inCodeBlock = true;
                }
            }
            if (codeBuilder.Length > 0)
                scripts.Add(codeBuilder.ToString());

            return scripts.ToArray();
        }
    }
}
