﻿namespace CentralSnippetsTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CentralSnippets.Main.Preview("Demos/HelloWorld.cs");
            CentralSnippets.Main.Download("Demos/HelloWorld.cs", "DownloadedHelloWorld.cs");
        }
    }
}