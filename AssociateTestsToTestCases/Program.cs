using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AssociateTestsToTestCases.Access.Association;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.TestCase;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Utility;
using CommandLine;
using Microsoft.TeamFoundation.Common;
using FileAccess = AssociateTestsToTestCases.Access.File.FileAccess;

namespace AssociateTestsToTestCases
{
    internal static class Program
    {
        private static Messages _messages;
        private static Associator _associator;
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
            _messages = new Messages();
            _fileAccessor = new FileAccess();

            ParseArguments(args);

            _associator = new Associator(_messages, _verboseLogging);

            GetTestmethods();

            GetVstsTestCases();

            //ResetStatusTestCases(); TODO

            Associate();

            OutputSummary();
        }

        private static void ParseArguments(string[] args)
        {
            Writer.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.Arguments.Status);
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
            Writer.WriteToConsole(_messages, _messages.Types.Success, _messages.Stage.Arguments.Success);
        }

        private static void GetTestmethods()
        {
            Writer.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.DllTestMethods.Status);
            _testMethods = _fileAccessor.ListTestMethods(_testAssemblyPaths);

            if (_testMethods.IsNullOrEmpty())
            {
                Writer.WriteToConsole(_messages, _messages.Types.Error, _messages.Stage.DllTestMethods.Failure);
                Environment.Exit(-1);
            }
            Writer.WriteToConsole(_messages, _messages.Types.Success, _messages.Stage.DllTestMethods.Success);
        }

        private static void GetVstsTestCases()
        {
            Writer.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.TestCases.Status);
            _testCases = _testCaseAccess.GetVstsTestCases();

            if (_testCases.IsNullOrEmpty())
            {
                Writer.WriteToConsole(_messages, _messages.Types.Error, _messages.Stage.TestCases.Failure);
                Environment.Exit(-1);
            }
            Writer.WriteToConsole(_messages, _messages.Types.Success, _messages.Stage.TestCases.Success);
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
            Writer.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.Association.Status);
            _associator.Associate(_testMethods, _testCases, _testCaseAccess, _validationOnly, _testType);
            Writer.WriteToConsole(_messages, _messages.Types.Success, _messages.Stage.Association.Success);
        }

        private static void OutputSummary()
        {
            Writer.WriteToConsole(_messages, _messages.Types.Stage, _messages.Stage.Summary.Status);
            Writer.WriteToConsole(_messages, _messages.Types.Summary, string.Format(_messages.Stage.Summary.Detailed, Counter.Success, Counter.Error, Counter.WarningMissingId + Counter.WarningTestMethodNotAvailable + Counter.WarningNoCorrespondingTestMethod, Counter.WarningMissingId, Counter.WarningTestMethodNotAvailable, Counter.WarningNoCorrespondingTestMethod));
            Writer.WriteToConsole(_messages, _messages.Types.Summary, string.Format(_messages.Stage.Summary.Overview, _testCases.Count, Counter.Total));
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
