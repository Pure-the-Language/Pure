﻿namespace CentralSnippetsTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CentralSnippets.Main.DisableSSLCheck();
            CentralSnippets.Main.Preview("Demos/HelloWorld.cs");
        }
    }
}