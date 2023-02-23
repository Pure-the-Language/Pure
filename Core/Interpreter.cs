using Console = Colorful.Console;

namespace Core
{
    public class Interpreter
    {
        public void Start(string welcomeMessage = null, bool advancedInterpretingMode = false, bool defaultPackages = true, string[] startingScripts = null, bool skipInteractiveMode = false)
        {
            if (!string.IsNullOrWhiteSpace(welcomeMessage))
                Console.WriteLine(welcomeMessage);

            var context = new RoslynContext(true);
            if (startingScripts != null)
                foreach (var script in startingScripts)
                    context.Evaluate(script);

            if (skipInteractiveMode)
                return;
            while (true)
            {
                Console.Write(">>> ");
                string input = Console.ReadLine().Trim();

                if (input == "exit" || input == "exit()")
                    return;
                context.Evaluate(input);
            }
        }
    }
}