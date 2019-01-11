using System;
using System.Linq;
using CommandLine;
using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Message;
using Microsoft.TeamFoundation.Core.WebApi;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Manager.File;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Common;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Manager.DevOps;
using AssociateTestsToTestCases.Manager.Output;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

using TestMethod = AssociateTestsToTestCases.Manager.File.TestMethod;

namespace AssociateTestsToTestCases
{
    internal static class Program
    {
        private const string SystemTeamProjectName = "SYSTEM_TeamProject";
        private const string PressAnyKeyToCloseWindowName = "\nPress any key to close the window...";
        private const string SequenceContainsNoMatchingElementName = "Sequence contains no matching element";

        private static InputOptions _inputOptions;

        private static IFileAccess _fileAccess;
        private static IDevOpsAccess _devOpsAccess;
        private static IOutputAccess _commandLineAccess;

        private static IFileManager _fileManager;
        private static IOutputManager _outputManager;
        private static IDevOpsManager _devOpsManager;

        private static Messages _messages;
        private static TestMethod[] _testMethods;
        private static TestCase[] _testCases;

        private static bool _isLocal;
        private static Counter.Counter _counter;

        private static void Main(string[] args)
        {
            try
            {
                Init(args);

                _outputManager.WriteToConsole(_messages.Stages.Project.Status, _messages.Types.Stage);

                if (_fileManager.TestMethodsPathIsEmpty() && _devOpsManager.TestPlanIsEmpty())
                {
                    _outputManager.WriteToConsole(_messages.Stages.Project.Failure, _messages.Types.Warning);
                }
                else
                {
                    _outputManager.WriteToConsole(_messages.Stages.Project.Success, _messages.Types.Success);

                    _testMethods = _fileManager.GetTestMethods();
                    _testCases = _devOpsManager.GetTestCases();
                    _devOpsManager.Associate(_testMethods, _testCases);
                }

                _outputManager.OutputSummary(_testMethods, _testCases);
            }
            catch
            {
                if (!_isLocal)
                {
                    Environment.ExitCode = -1;
                }
            }

            if (_isLocal)
            {
                Console.ResetColor();
                Console.Write(PressAnyKeyToCloseWindowName);
                Console.ReadKey();
            }
        }

        private static void Init(string[] args)
        {
            _messages = new Messages();
            _counter = new Counter.Counter();
            _inputOptions = new InputOptions();
            _isLocal = Environment.GetEnvironmentVariable(SystemTeamProjectName) == null;

            InitAccesses(args);
            InitManagers();
        }

        private static void InitAccesses(string[] args)
        {
            _commandLineAccess = new CommandLineAccess(_isLocal, _messages, new AzureDevOpsColors());
            ParseArguments(args);

            _fileAccess = new FileAccess(new AssemblyHelper());
            _inputOptions.TestAssemblyPaths = _fileAccess.ListTestAssemblyPaths(_inputOptions.Directory, _inputOptions.MinimatchPatterns);

            var httpClients = RetrieveHttpClients(new VssConnection(new Uri(_inputOptions.CollectionUri), new VssBasicCredential(string.Empty, _inputOptions.PersonalAccessToken)));
            var workItemTrackingHttpClient = httpClients.Item1;
            var testManagementHttpClient = httpClients.Item2;

            ValidateDevOpsCredentials(testManagementHttpClient);
            _devOpsAccess = new AzureDevOpsAccess(testManagementHttpClient, workItemTrackingHttpClient, _messages, _commandLineAccess, _inputOptions, _counter);
        }

        private static void InitManagers()
        {
            _outputManager = new OutputManager(_messages, _commandLineAccess, _counter);
            _fileManager = new FileManager(_messages, _fileAccess, _commandLineAccess, _inputOptions);
            _devOpsManager = new AzureDevOpsManager(_messages, _outputManager, _devOpsAccess, _counter);
        }

        private static void ParseArguments(string[] args)
        {
            _commandLineAccess.WriteToConsole(_messages.Stages.Argument.Status, _messages.Types.Stage);

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    _inputOptions.TestType = o.TestType;
                    _inputOptions.ProjectName = o.ProjectName;
                    _inputOptions.TestPlanName = o.TestPlanName;
                    _inputOptions.CollectionUri = o.CollectionUri;
                    _inputOptions.ValidationOnly = o.ValidationOnly;
                    _inputOptions.VerboseLogging = o.VerboseLogging;
                    _inputOptions.Directory = o.Directory;
                    _inputOptions.PersonalAccessToken = o.PersonalAccessToken;
                    _inputOptions.MinimatchPatterns = o.MinimatchPatterns.Split(';').Select(s => s.ToLowerInvariant()).ToArray();
                });

            _commandLineAccess.WriteToConsole(_messages.Stages.Argument.Success, _messages.Types.Success);
        }

        private static (WorkItemTrackingHttpClient, TestManagementHttpClient) RetrieveHttpClients(VssConnection connection)
        {
            _commandLineAccess.WriteToConsole(_messages.Stages.HttpClient.Status, _messages.Types.Stage);

            WorkItemTrackingHttpClient workItemTrackingHttpClient;
            TestManagementHttpClient testManagementHttpClient;

            try
            {
                testManagementHttpClient = connection.GetClient<TestManagementHttpClient>();
                workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            }
            catch (Exception e)
            {
                var innerException = e.InnerException ?? e;

                if (innerException.GetType().Equals(typeof(VssServiceResponseException)))
                {
                    _commandLineAccess.WriteToConsole(string.Format(_messages.Stages.HttpClient.FailureResourceNotFound, _inputOptions.CollectionUri), _messages.Types.Error);

                    throw new InvalidOperationException();
                }
                else if (innerException.GetType().Equals(typeof(VssServiceException)))
                {
                    _commandLineAccess.WriteToConsole(string.Format(_messages.Stages.HttpClient.FailureUserNotAuthorized, _inputOptions.CollectionUri), _messages.Types.Error);

                    throw new InvalidOperationException();
                }

                // Unknown error - output debug message
                _commandLineAccess.WriteToConsole(innerException.Message, _messages.Types.Error);
                throw e;
            }

            _commandLineAccess.WriteToConsole(_messages.Stages.HttpClient.Success, _messages.Types.Success);
            return (workItemTrackingHttpClient, testManagementHttpClient);
        }

        private static void ValidateDevOpsCredentials(TestManagementHttpClient testManagementHttpClient)
        {
            _commandLineAccess.WriteToConsole(_messages.Stages.DevOpsCredentials.Status, _messages.Types.Stage);

            try
            {
                testManagementHttpClient.GetPlansAsync(_inputOptions.ProjectName).Result
                .Single(x => x.Name.Equals(_inputOptions.TestPlanName));
            }
            catch (Exception e)
            {
                var innerException = e.InnerException ?? e;

                if (innerException.GetType().Equals(typeof(VssUnauthorizedException)))
                {
                    _commandLineAccess.WriteToConsole(string.Format(_messages.Stages.DevOpsCredentials.FailureUserNotAuthorized, _inputOptions.CollectionUri), _messages.Types.Error);

                    throw new InvalidOperationException();
                }
                else if (innerException.GetType().Equals(typeof(ProjectDoesNotExistWithNameException)))
                {
                    _commandLineAccess.WriteToConsole(string.Format(_messages.Stages.DevOpsCredentials.FailureNonExistingProject, _inputOptions.ProjectName), _messages.Types.Error);

                    throw new InvalidOperationException();
                }
                else if (innerException.GetType().Equals(typeof(InvalidOperationException)) && e.Message.Equals(SequenceContainsNoMatchingElementName))
                {
                    _commandLineAccess.WriteToConsole(string.Format(_messages.Stages.DevOpsCredentials.FailureNonExistingTestPlan, _inputOptions.TestPlanName), _messages.Types.Error);

                    throw new InvalidOperationException();
                }

                // Unknown error - output debug message
                _commandLineAccess.WriteToConsole(innerException.Message, _messages.Types.Error);
                throw innerException;
            }

            _commandLineAccess.WriteToConsole(_messages.Stages.DevOpsCredentials.Success, _messages.Types.Success);
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

            [Option('l', "verboselogging", Required = false, HelpText = "When Verbose logging is turned on it also outputs the successful matchings and the fixes next to the warnings/errors.")]
            public bool VerboseLogging { get; set; }
        }
    }
}
