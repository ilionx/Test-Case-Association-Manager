using System;
using System.Linq;
using CommandLine;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Common;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Utility;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Access.TestCase;

namespace AssociateTestsToTestCases
{
    internal static class Program
    {
        private static List<TestCase> _testCases;
        private static MethodInfo[] _testMethods;

        private static Messages _messages;
        private static FileAccess _fileAccessor;
        private static TestCaseAccess _testCaseAccess;

        private static bool _validationOnly;
        private static bool _verboseLogging;
        private static string[] _testAssemblyPaths;
        private static string _testType = string.Empty;

        private static void Main(string[] args)
        {
            Init(args);

            GetTestMethods();

            GetTestCases();

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
            CommandLineAccess.WriteToConsole(_messages, _messages.Stages.Argument.Status, _messages.Types.Stage);
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
            CommandLineAccess.WriteToConsole(_messages, _messages.Stages.Argument.Success, _messages.Types.Success);
        }

        private static void GetTestMethods()
        {
            CommandLineAccess.WriteToConsole(_messages, _messages.Stages.TestMethod.Status, _messages.Types.Stage);
            _testMethods = _fileAccessor.ListTestMethods(_testAssemblyPaths);
            if (_testMethods.IsNullOrEmpty())
            {
                CommandLineAccess.WriteToConsole(_messages, _messages.Stages.TestMethod.Failure, _messages.Types.Error);
                Environment.Exit(-1);
            }
            CommandLineAccess.WriteToConsole(_messages, string.Format(_messages.Stages.TestMethod.Success, _testMethods.Length), _messages.Types.Success);
        }

        private static void GetTestCases()
        {
            CommandLineAccess.WriteToConsole(_messages, _messages.Stages.TestCase.Status, _messages.Types.Stage);
            _testCases = _testCaseAccess.GetTestCases();

            if (_testCases.IsNullOrEmpty())
            {
                CommandLineAccess.WriteToConsole(_messages, _messages.Stages.TestCase.Failure, _messages.Types.Error);
                Environment.Exit(-1);
            }
            CommandLineAccess.WriteToConsole(_messages, string.Format(_messages.Stages.TestCase.Success, _testCases.Count), _messages.Types.Success);
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
            CommandLineAccess.WriteToConsole(_messages, _messages.Stages.Association.Status, _messages.Types.Stage);
            var testMethodsNotMapped = new AzureDevOpsAccess(_messages, _verboseLogging).Associate(_testMethods, _testCases, _testCaseAccess, _validationOnly, _testType);

            if (testMethodsNotMapped.Count != 0)
            {
                CommandLineAccess.WriteToConsole(_messages, string.Format(_messages.Stages.Association.Failure, testMethodsNotMapped.Count), _messages.Types.Error);
                testMethodsNotMapped.ForEach(testMethod => CommandLineAccess.WriteToConsole(_messages, $"{testMethod.FullName}.{testMethod.Name}", _messages.Types.Info));
                CommandLineAccess.WriteToConsole(_messages, string.Empty);

                OutputSummary();

                Environment.Exit(-1);
            }
            CommandLineAccess.WriteToConsole(_messages, string.Format(_messages.Stages.Association.Success, Counter.Success), _messages.Types.Success);
        }

        private static void OutputSummary()
        {
            CommandLineAccess.WriteToConsole(_messages, _messages.Stages.Summary.Status, _messages.Types.Stage);
            CommandLineAccess.WriteToConsole(_messages, string.Format(_messages.Stages.Summary.Detailed, Counter.Success, Counter.Error, Counter.WarningMissingId + Counter.WarningTestMethodNotAvailable + Counter.WarningNoCorrespondingTestMethod, Counter.WarningMissingId, Counter.WarningTestMethodNotAvailable, Counter.WarningNoCorrespondingTestMethod), _messages.Types.Summary);
            CommandLineAccess.WriteToConsole(_messages, string.Format(_messages.Stages.Summary.Overview, _testCases.Count, _testMethods.Length, Counter.Total), _messages.Types.Summary);
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
