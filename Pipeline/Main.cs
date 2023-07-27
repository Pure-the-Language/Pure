using System.Diagnostics;

namespace Pipeline
{
    public static class PathHelper
    {
        public static string FindProgram(string program)
        {
            if (File.Exists(Path.GetFullPath(program))) return Path.GetFullPath(program);

            string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
            return paths
                .SelectMany(folder =>
                {
                    if (program.ToLower().EndsWith(".exe"))
                        return new[] { Path.Combine(folder, program) };
                    else
                        return new[] { Path.Combine(folder, program), Path.Combine(folder, program + ".exe") };
                })
                .FirstOrDefault(File.Exists);
        }
    }

    public static class Main
    {
        public static string Run(string program)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program
                }
            };
            p.Start();
            
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        public static string Run(string program, string arguments)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = arguments
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        public static string Run(string program, params string[] arguments)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = string.Join(" ", arguments.Select(EscapeArgument))
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        public static string Run(string program, Dictionary<string, string> arguments)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = string.Join(" ", arguments.Select(p => $"{p.Key} {EscapeArgument(p.Value)}"))
                }
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }

        #region Helpers
        private static string EscapeArgument(string original)
        {
            if (original.Contains('"'))
                return "\"" + original.Replace("\"", "\\\"") + "\"";
            else if(original.Contains(' '))
                return "\"" + original + "\"";
            else
                return original;
        }
        #endregion
    }
}