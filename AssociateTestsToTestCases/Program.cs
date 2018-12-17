using System;
using System.Linq;
using CommandLine;
using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Manager.File;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Common;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.DevOps;
using AssociateTestsToTestCases.Manager.Output;
using AssociateTestsToTestCases.Access.TestCase;
using AssociateTestsToTestCases.Manager.TestCase;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace AssociateTestsToTestCases
{
    internal static class Program
    {
        private const string SystemTeamProjectName = "SYSTEM_TeamProject";

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

        private static Messages _messages;
        private static MethodInfo[] _testMethods;
        private static List<TestCase> _testCases;

        private static void Main(string[] args)
        {
            try
            {
                Init(args);

                _testMethods = _fileManager.GetTestMethods(_testAssemblyPaths);
                _testCases = _testCaseManager.GetTestCases();
                _azureDevOpsManager.Associate(_testMethods, _testCases, _validationOnly, _testType);

                _outputManager.OutputSummary(_testMethods, _testCases);
            }
            catch
            {
                Environment.ExitCode = -1;
            }
        }

        private static void Init(string[] args)
        {
            var isLocal = Environment.GetEnvironmentVariable(SystemTeamProjectName) == null;

            _messages = new Messages();
            _commandLineAccess = new CommandLineAccess(isLocal, _messages, new AzureDevOpsColors());
            ParseArguments(args);

            _fileAccess = new FileAccess(new AssemblyHelper());
            _azureDevOpsAccess = new AzureDevOpsAccess(_messages, _commandLineAccess, _verboseLogging);

            var connection = new VssConnection(new Uri(_collectionUri), new VssBasicCredential(string.Empty, _personalAccessToken));
            var testManagementHttpClient = connection.GetClient<TestManagementHttpClient>();
            var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            _testCaseAccess = new TestCaseAccess(testManagementHttpClient, workItemTrackingHttpClient, _testPlanName, _projectName);

            _outputManager = new OutputManager(_messages, _commandLineAccess);
            _fileManager = new FileManager(_messages, _fileAccess, _commandLineAccess);
            _testCaseManager = new TestCaseManager(_messages, _commandLineAccess, _testCaseAccess);
            _azureDevOpsManager = new AzureDevOpsManager(_messages, _commandLineAccess, _outputManager, _testCaseAccess, _azureDevOpsAccess);

            _testAssemblyPaths = _fileAccess.ListTestAssemblyPaths(_directory, _minimatchPatterns);
        }

        private static void ParseArguments(string[] args)
        {
           _commandLineAccess.WriteToConsole(_messages.Stages.Argument.Status, _messages.Types.Stage);
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
           _commandLineAccess.WriteToConsole(_messages.Stages.Argument.Success, _messages.Types.Success);
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
