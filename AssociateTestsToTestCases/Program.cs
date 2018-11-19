using System;
using System.Linq;
using CommandLine;
using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Event;
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
        private static FileAccess _fileAccess;
        private static TestCaseAccess _testCaseAccess;
        private static AzureDevOpsAccess _azureDevOpsAccess;

        private static bool _validationOnly;
        private static bool _verboseLogging;
        private static string[] _testAssemblyPaths;
        private static string _testType = string.Empty;

        private static CommandLineAccess _commandLineAccess;
        private static WriteToConsoleEventLogger _writeToConsoleEventLogger;

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
            _fileAccess = new FileAccess();
            _writeToConsoleEventLogger = new WriteToConsoleEventLogger();
            _commandLineAccess = new CommandLineAccess(new Messages(), new AzureDevOpsColors());

            SubscribeMethods();

            ParseArguments(args);
            _azureDevOpsAccess = new AzureDevOpsAccess(_writeToConsoleEventLogger, _messages, _verboseLogging);
        }

        private static void SubscribeMethods()
        {
            _writeToConsoleEventLogger.WriteToConsole += _commandLineAccess.OnWriteToConsole;
        }

        private static void ParseArguments(string[] args)
        {
            _writeToConsoleEventLogger.Write(_messages.Stages.Argument.Status, _messages.Types.Stage);
            Parser.Default.ParseArguments<Program.Options>(args)
                .WithParsed(o =>
                {
                    var minimatchPatterns = o.MinimatchPatterns.Split(';').Select(s => s.ToLowerInvariant()).ToArray();
                    var directory = o.Directory.ToLowerInvariant();
                    _testAssemblyPaths = _fileAccess.ListTestAssemblyPaths(directory, minimatchPatterns);

                    _testCaseAccess = new TestCaseAccess(o.CollectionUri, o.PersonalAccessToken, o.ProjectName, o.TestPlanName);
                    _validationOnly = o.ValidationOnly;
                    _testType = o.TestType;
                    _verboseLogging = o.VerboseLogging;
                });
            _writeToConsoleEventLogger.Write(_messages.Stages.Argument.Success, _messages.Types.Success);
        }

        private static void GetTestMethods()
        {
            _writeToConsoleEventLogger.Write(_messages.Stages.TestMethod.Status, _messages.Types.Stage);
            _testMethods = _fileAccess.ListTestMethods(_testAssemblyPaths);
            if (_testMethods.IsNullOrEmpty())
            {
                _writeToConsoleEventLogger.Write(_messages.Stages.TestMethod.Failure, _messages.Types.Error);
                Environment.Exit(-1);
            }

            var duplicateTestMethods = _fileAccess.ListDuplicateTestMethods(_testMethods);
            if (duplicateTestMethods.Count != 0)
            {
                _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.TestMethod.Duplicate, duplicateTestMethods.Count), _messages.Types.Error);
                duplicateTestMethods.ForEach(x => _writeToConsoleEventLogger.Write(string.Format(_messages.TestMethods.Duplicate, x.Name, _messages.TestMethods.GetDuplicateTestMethodNamesString(x.TestMethods)), _messages.Types.Info));
                Environment.Exit(-1);
            }

            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.TestMethod.Success, _testMethods.Length),_messages.Types.Success);
        }

        private static void GetTestCases()
        {
            _writeToConsoleEventLogger.Write(_messages.Stages.TestCase.Status, _messages.Types.Stage);
            _testCases = _testCaseAccess.GetTestCases();

            if (_testCases.IsNullOrEmpty())
            {
                _writeToConsoleEventLogger.Write(_messages.Stages.TestCase.Failure, _messages.Types.Error);
                Environment.Exit(-1);
            }

            var duplicateTestCases = _testCaseAccess.ListDuplicateTestCases(_testCases);
            if (duplicateTestCases.Count != 0)
            {
                _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.TestCase.Duplicate, duplicateTestCases.Count), _messages.Types.Error);
                duplicateTestCases.ForEach(x => _writeToConsoleEventLogger.Write(string.Format(_messages.TestCases.Duplicate, x.Name, _messages.TestCases.GetDuplicateTestCaseNamesString(x.TestCases)), _messages.Types.Info));
                Environment.Exit(-1);
            }

            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.TestCase.Success, _testCases.Count), _messages.Types.Success);
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
            _writeToConsoleEventLogger.Write(_messages.Stages.Association.Status, _messages.Types.Stage);

            var testMethods = _testMethods.Select(x => new TestMethod(x.Name, x.Module.Name, x.DeclaringType.FullName)).ToList();

            var testMethodsNotAvailable = _azureDevOpsAccess.ListTestCasesWithNotAvailableTestMethods(_testCases, testMethods);
            if (testMethodsNotAvailable.Count != 0)
            {
               testMethodsNotAvailable.ForEach(x => _writeToConsoleEventLogger.Write(string.Format(_messages.Associations.TestCaseWithNotAvailableTestMethod, x.Title, x.Id, x.AutomatedTestName), _messages.Types.Warning, _messages.Reasons.AssociatedTestMethodNotAvailable));
            }

            var testMethodsAssociationErrorCount = _azureDevOpsAccess.Associate(testMethods, _testCases, _testCaseAccess, _validationOnly, _testType);
            if (testMethodsAssociationErrorCount != 0)
            {
                _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.Association.Failure, testMethodsAssociationErrorCount), _messages.Types.Error);

                OutputSummary();

                Environment.Exit(-1);
            }

            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.Association.Success, Counter.Success), _messages.Types.Success);
        }

        private static void OutputSummary()
        {
            _writeToConsoleEventLogger.Write(_messages.Stages.Summary.Status, _messages.Types.Stage);
            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.Summary.Detailed, Counter.Success, Counter.FixedReference, Counter.Error, Counter.OperationFailed, Counter.TestCaseNotFound, Counter.Warning, Counter.TestMethodNotAvailable), _messages.Types.Summary);
            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.Summary.Overview, _testCases.Count, _testMethods.Length, Counter.Total), _messages.Types.Summary);
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
