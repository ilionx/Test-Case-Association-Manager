using System.Linq;
using CommandLine;
using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Manager.File;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.DevOps;
using AssociateTestsToTestCases.Manager.Output;
using AssociateTestsToTestCases.Access.TestCase;
using AssociateTestsToTestCases.Manager.TestCase;

namespace AssociateTestsToTestCases
{
    internal static class Program
    {
        private static bool _validationOnly;
        private static bool _verboseLogging;

        private static string _testType;
        private static string _directory;
        private static string _projectName;
        private static string _collectionUri;
        private static string _testPlanName;
        private static string _personalAccessToken;

        private static string[] _minimatchPatterns;
        private static string[] _testAssemblyPaths;

        private static IFileAccess _fileAccess;
        private static ITestCaseAccess _testCaseAccess;
        private static IDevOpsAccess _azureDevOpsAccess;
        private static IOutputAccess _commandLineAccess;

        private static IFileManager _fileManager;
        private static IOutputManager _outputManager;
        private static ITestCaseManager _testCaseManager;
        private static IDevOpsManager _azureDevOpsManager;

        private static IWriteToConsoleEventLogger _writeToConsoleEventLogger;

        private static Messages _messages;
        private static MethodInfo[] _testMethods;
        private static List<TestCase> _testCases;

        private static void Main(string[] args)
        {
            Init(args);

            _testMethods = _fileManager.GetTestMethods(_testAssemblyPaths);
            _testCases = _testCaseManager.GetTestCases();
            //ResetStatusTestCases(); TODO
            _azureDevOpsManager.Associate(_testMethods, _testCases, _validationOnly, _testType);

            _outputManager.OutputSummary(_testMethods, _testCases);
        }

        private static void Init(string[] args)
        {
            _messages = new Messages();
            _writeToConsoleEventLogger = new WriteToConsoleEventLogger();
            _commandLineAccess = new CommandLineAccess(_messages, new AzureDevOpsColors()); //Todo: fix this: use factory!
            SubscribeMethods();

            ParseArguments(args);

            _fileAccess = new FileAccess(new AssemblyHelper()); //Todo: fix this: use factory!
            _azureDevOpsAccess = new AzureDevOpsAccess(_writeToConsoleEventLogger, _messages, _verboseLogging); //Todo: fix this: use factory!
            _testCaseAccess = new TestCaseAccess(_collectionUri, _personalAccessToken, _projectName, _testPlanName); //Todo: fix this: use factory!

            _outputManager = new OutputManager(_messages, _writeToConsoleEventLogger); //Todo: fix this: use factory!
            _fileManager = new FileManager(_messages, _fileAccess, _writeToConsoleEventLogger); //Todo: fix this: use factory!
            _testCaseManager = new TestCaseManager(_messages, _testCaseAccess, _writeToConsoleEventLogger); //Todo: fix this: use factory!
            _azureDevOpsManager = new AzureDevOpsManager(_messages, _outputManager, _testCaseAccess, _azureDevOpsAccess, _writeToConsoleEventLogger); //Todo: fix this: use factory!

            _testAssemblyPaths = _fileAccess.ListTestAssemblyPaths(_directory, _minimatchPatterns);
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
                    _testType = o.TestType;
                    _projectName = o.ProjectName;
                    _testPlanName = o.TestPlanName;
                    _collectionUri = o.CollectionUri;
                    _validationOnly = o.ValidationOnly;
                    _verboseLogging = o.VerboseLogging;
                    _directory = o.Directory.ToLowerInvariant();
                    _personalAccessToken = o.PersonalAccessToken;
                    _minimatchPatterns = o.MinimatchPatterns.Split(';').Select(s => s.ToLowerInvariant()).ToArray();
                });
            _writeToConsoleEventLogger.Write(_messages.Stages.Argument.Success, _messages.Types.Success);
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
