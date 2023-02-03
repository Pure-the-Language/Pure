using Microsoft.CodeAnalysis.Scripting;
using Console = Colorful.Console;

namespace Core
{
    public class Interpreter
    {
        public void Start(string welcomeMessage = null, bool advancedInterpretingMode = false, bool defaultPackages = true)
        {
            if (welcomeMessage != null)
                Console.WriteLine(welcomeMessage);

            var context = new RoslynContext(ScriptOptions.Default.WithImports("System.Math"));
            while (true)
            {
                Console.Write(">>> ");
                string input = Console.ReadLine();

                context.Evaluate(input);
            }
        }
    }
}