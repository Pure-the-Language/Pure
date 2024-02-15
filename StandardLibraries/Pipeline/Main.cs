using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Pipeline
{
    /// <summary>
    /// Main pipeline interface
    /// </summary>
    public sealed class Pipeline
    {
        #region Construction
        /// <summary>
        /// Return result from previous invokation
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Construct a new pipeline with content at this step
        /// </summary>
        public Pipeline(string content = null)
        {
            Content = content;
        }
        #endregion

        #region Main Interface Method (Fluent API)
        /// <summary>
        /// Continue or running the pipeline with next invokation
        /// </summary>
        public Pipeline Pipe(string program, string arguments)
        {
            if (Content != null)
            {
                string output = Main.Feed(program, arguments, Content);
                return new Pipeline(output);
            }
            else
                return Main.Pipe(program, arguments);
        }
        #endregion

        #region Operator Overloading
        /// <summary>
        /// Feed pipeline using strings
        /// </summary>
        /// <param name="nextStep">Complete program name plus arguments</param>
        /// <returns>Finished pipe</returns>
        public static Pipeline operator |(Pipeline step, string nextStep)
        {
            if (nextStep.StartsWith('"'))
            {
                // Get program name and arguments
                string program = Regex.Match(nextStep, @"^""(.*?)""").Groups[1].Value;
                string arguments = nextStep.Substring(program.Length).Trim();
                string output = Main.Feed(program, arguments, step.Content);
                return new Pipeline(output);
            }
            else
            {
                // Get program name and arguments
                string program = nextStep.Split(' ')[0];
                string arguments = nextStep.Substring(program.Length).Trim();
                string output = Main.Feed(program, arguments, step.Content);
                return new Pipeline(output);
            }
        }
        #endregion
    }
    /// <summary>
    /// Library main exposed interface
    /// </summary>
    public static class Main
    {
        #region Helper Class
        /// <summary>
        /// Helpers in identifying executable location
        /// </summary>
        public static class PathHelper
        {
            /// <summary>
            /// Find disk location of program
            /// </summary>
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
        #endregion

        #region Run Methods
        /// <summary>
        /// Run program
        /// </summary>
        public static string Run(string program)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
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
        /// <summary>
        /// Run program with arguments (final form)
        /// </summary>
        public static string Run(string program, string arguments)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
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
        /// <summary>
        /// Run program with arguments as array
        /// </summary>
        public static string Run(string program, params string[] arguments)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
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
        /// <summary>
        /// Run program with arguments as dictionary
        /// </summary>
        public static string Run(string program, Dictionary<string, string> arguments)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
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
        #endregion

        #region Feeding Method
        /// <summary>
        /// Run program and feed standard input from string
        /// </summary>
        public static string Feed(string program, string arguments, string standardInput)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = arguments
                }
            };
            p.Start();

            StreamWriter redirectStreamWriter = p.StandardInput;
            redirectStreamWriter.WriteLine(standardInput);
            redirectStreamWriter.Close();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
        #endregion

        #region Piping Methods
        /// <summary>
        /// Start a pipeline with program
        /// </summary>
        public static Pipeline Pipe(string program)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
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

            return new Pipeline(output);
        }
        /// <summary>
        /// Start a pipeline with program and arguments
        /// </summary>
        public static Pipeline Pipe(string program, string arguments)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
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

            return new Pipeline(output);
        }
        /// <summary>
        /// Start a pipeline with program and arguments in array format
        /// </summary>
        public static Pipeline Pipe(string program, params string[] arguments)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
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

            return new Pipeline(output);
        }
        /// <summary>
        /// Start a pipeline program and arguments in dictionary format
        /// </summary>
        public static Pipeline Pipe(string program, Dictionary<string, string> arguments)
        {
            string programPath = PathHelper.FindProgram(program);
            if (programPath == null)
                throw new ArgumentException($"Cannot find program {program}.");

            using Process p = new()
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

            return new Pipeline(output);
        }
        #endregion

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