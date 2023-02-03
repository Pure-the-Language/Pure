using Core;

namespace Pure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Interpreter().Start("""
                Pure v0.0.1
                """);
        }
    }
}