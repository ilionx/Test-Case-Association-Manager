using System;
using CommandLine;

namespace AssociateTestsToTestCases
{
    class Program
    {
        public class Options
        {
            [Option('f', "test files", Required = true, HelpText = "Supports multiple minimatch patterns, separated by a semicolon.")]
            public string TestFiles { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                var minimatchPatterns = o.TestFiles.Split(';');

            });

            Console.WriteLine("Hello World!");
        }
    }
}
