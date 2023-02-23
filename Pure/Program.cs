using Core;

namespace Pure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                new Interpreter().Start("""
                    Pure v0.0.1
                    """);
            else if (args.Length == 1) 
            {
                string file = Path.GetFullPath(args[0]);
                if (!File.Exists(file))
                {
                    Console.WriteLine($"File {file} doesn't exist.");
                    return; 
                }
                // TODO: Currently we are NOT properly parsing the file - we assume only line commands
                new Interpreter().Start(string.Empty, false, true, File.ReadAllLines(file), true);
            }
        }
    }
}