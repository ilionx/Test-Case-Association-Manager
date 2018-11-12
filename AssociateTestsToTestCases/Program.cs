using System;
using CommandLine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Common;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Utility;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.TestCase;
using AssociateTestsToTestCases.Access.AzureDevOps;

namespace AssociateTestsToTestCases
{
    internal static class Program
    {
        private static Messages _messages;
        private static FileAccess _fileAccessor;
        private static TestCaseAccess _testCaseAccess;

        private static bool _validationOnly;
        private static bool _verboseLogging;
        private static string[] _testAssemblyPaths;
        private static string _testType = string.Empty;

        private static List<TestCase> _testCases;
        private static MethodInfo[] _testMethods;

        private static void Main(string[] args)
        {
            Init(args);

            GetTestmethods();

            GetVstsTestCases();

            //ResetStatusTestCases(); TODO

            Associate();

            OutputSummary();
        }

        private static void Init(string[] args)
        {
            _messages = new Messages();
            _fileAccessor = new FileAccess();

            ParseArguments(args);
        }

        private static void ParseArguments(string[] args)
        {
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.Arguments.Status);
            Parser.Default.ParseArguments<Program.Options>(args)
                .WithParsed(o =>
                {
                    var minimatchPatterns = o.MinimatchPatterns.Split(';').Select(s => s.ToLowerInvariant()).ToArray();
                    var directory = o.Directory.ToLowerInvariant();
                    _testAssemblyPaths = _fileAccessor.ListTestAssemblyPaths(directory, minimatchPatterns);

                    _testCaseAccess = new TestCaseAccess(o.CollectionUri, o.PersonalAccessToken, o.ProjectName, o.TestPlanName);
                    _validationOnly = o.ValidationOnly;
                    _testType = o.TestType;
                    _verboseLogging = o.VerboseLogging;
                });
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Success, _messages.Stage.Arguments.Success);
        }

        private static void GetTestmethods()
        {
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.DllTestMethods.Status);
            _testMethods = _fileAccessor.ListTestMethods(_testAssemblyPaths);
            if (_testMethods.IsNullOrEmpty())
            {
                CommandLineAccess.WriteToConsole(_messages, _messages.Types.Error, _messages.Stage.DllTestMethods.Failure);
                Environment.Exit(-1);
            }
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Success, string.Format(_messages.Stage.DllTestMethods.Success, _testMethods.Length));
        }

        private static void GetVstsTestCases()
        {
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.TestCases.Status);
            _testCases = _testCaseAccess.GetVstsTestCases();

            if (_testCases.IsNullOrEmpty())
            {
                CommandLineAccess.WriteToConsole(_messages, _messages.Types.Error, _messages.Stage.TestCases.Failure);
                Environment.Exit(-1);
            }
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Success, string.Format(_messages.Stage.TestCases.Success, _testCases.Count));
        }

        //private static void ResetStatusTestCases()
        //{
        //    //Console.WriteLine("Trying to reset the status of each test case");
        //    //var resetStatusTestCasesSuccess = testCaseAccess.ResetStatusTestCases();

        //    //if (!resetStatusTestCasesSuccess)
        //    //{
        //    //    Console.Write("[ERROR] Could not reset the status of each VSTS Test Case. Program has been terminated.\n");
        //    //    Environment.Exit(-1);
        //    //}
        //    //Console.WriteLine("[SUCCESS] VSTS Test Cases have been reset.\n");
        //}

        private static void Associate()
        {
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.Association.Status);
            var testMethodsNotMapped = new AzureDevOpsAccess(_messages, _verboseLogging).Associate(_testMethods, _testCases, _testCaseAccess, _validationOnly, _testType);

            if (testMethodsNotMapped.Count != 0)
            {
                CommandLineAccess.WriteToConsole(_messages, _messages.Types.Error, string.Format(_messages.Stage.Association.Failure, testMethodsNotMapped.Count));

                OutputSummary();

                CommandLineAccess.WriteToConsole(_messages, _messages.Types.Summary, _messages.Stage.Summary.TestCases);
                testMethodsNotMapped.ForEach(testMethod => CommandLineAccess.WriteToConsole(_messages, _messages.Types.Info, $"{testMethod.FullName}.{testMethod.Name}"));
                Environment.Exit(-1);
            }
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Success, string.Format(_messages.Stage.Association.Success, Counter.Success));
        }

        private static void OutputSummary()
        {
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.Summary.Status);
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Summary, string.Format(_messages.Stage.Summary.Detailed, Counter.Success, Counter.Error, Counter.WarningMissingId + Counter.WarningTestMethodNotAvailable + Counter.WarningNoCorrespondingTestMethod, Counter.WarningMissingId, Counter.WarningTestMethodNotAvailable, Counter.WarningNoCorrespondingTestMethod));
            CommandLineAccess.WriteToConsole(_messages, _messages.Types.Summary, string.Format(_messages.Stage.Summary.Overview, _testCases.Count, _testMethods.Length, Counter.Total));
        }

        private class Options
        {
            [Option('d', "directory", Required = true, HelpText = "The root directory to search in.")]
            public string Directory { get; set; }

            [Option('m', "minimatchpatterns", Required = true, HelpText = "Minimatch patterns to search for within the directory, separated by a semicolon.")]
            public string MinimatchPatterns { get; set; }

            [Option('p', "personalaccesstoken", Required = true, HelpText = "The personal access token used for accessing the Azure DevOps project.")]
            public string PersonalAccessToken { get; set; }

            [Option('u', "collectionuri", Required = true, HelpText = "The Azure DevOps collection Uri used for accessing the project test cases.")]
            public string CollectionUri { get; set; }

            [Option('n', "projectname", Required = true, HelpText = "The project name containing the test plan.")]
            public string ProjectName { get; set; }

            [Option('e', "testplanname", Required = true, HelpText = "The name of the test plan containing the test cases.")]
            public string TestPlanName { get; set; }

            [Option('t', "testtype", Required = false, Default = "", HelpText = "The automation test type.")]
            public string TestType { get; set; }

            [Option('v', "validationonly", Required = false, HelpText = "Indicates if you only want to validate the changes without saving the test cases.")]
            public bool ValidationOnly { get; set; }

            [Option('l', "verboselogging", Required = false, HelpText = "When Verbose logging is turned on it also outputs the successful matchings next to the warnings.")]
            public bool VerboseLogging { get; set; }
        }
    }
}
