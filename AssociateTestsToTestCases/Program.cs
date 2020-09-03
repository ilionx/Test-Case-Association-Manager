using System;
using System.Linq;
using System.Diagnostics;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Parsing;
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
using AssociateTestsToTestCases.Access.File.Strategy;

namespace AssociateTestsToTestCases
{
    internal static partial class Program
    {
        private const string SystemTeamProjectName = "SYSTEM_TeamProject";
        private const string PressAnyKeyToCloseWindowName = "\nPress any key to close the window...";
        private const string SequenceContainsNoMatchingElementName = "Sequence contains no matching element";

        private static InputOptions _inputOptions;
        private static string[] _testAssemblyPaths;

        private static IFileAccess _fileAccess;
        private static IDevOpsAccess _devOpsAccess;
        private static IOutputAccess _commandLineAccess;

        private static IFileManager _fileManager;
        private static IOutputManager _outputManager;
        private static IDevOpsManager _devOpsManager;

        private static Messages _messages;
        private static TestCase[] _testCases;
        private static TestMethod[] _testMethods;

        private static bool _isLocal;
        private static Counter.Counter _counter;
        private static AzureDevOpsColors _azureDevOpsColors;

        private static ITestFrameworkStrategy _testFrameWorkStrategy;

        private static void Main(string[] args)
        {
            try
            {
                InitializeProgram(args);
                InitializeTestAssemblyPaths();

                _testMethods = _fileManager.GetTestMethods(_testAssemblyPaths);
                _testCases = _devOpsManager.GetTestCases();
                _devOpsManager.Associate(_testMethods, _testCases);

                _outputManager.OutputSummary(_testMethods, _testCases);
            }
            catch (Exception e)
            {
                if (_inputOptions.DebugMode)
                {
                    WriteTraceToConsole(e);
                }

                if (!_isLocal)
                {
                    Environment.ExitCode = -1;
                }
            }

            if (_isLocal)
            {
                PromptUserToCloseWindow();
            }
        }

        private static void InitializeProgram(string[] args)
        {
            _messages = new Messages();
            _counter = new Counter.Counter();
            _azureDevOpsColors = new AzureDevOpsColors();
            _isLocal = Environment.GetEnvironmentVariable(SystemTeamProjectName) == null;
            _inputOptions = new CommandLineArgumentsParser(CreateCommandLineAccess(_isLocal, _messages, _azureDevOpsColors), _messages).Parse(args);
            _testFrameWorkStrategy = RetrieveTestFrameworkStrategies().Single(x => x.TestFrameworkType == Enum.Parse<TestFrameworkType>(_inputOptions.TestFrameworkType, true));

            InitializeAccesses();
            InitializeManagers();
        }

        private static ITestFrameworkStrategy[] RetrieveTestFrameworkStrategies()
        {
            // todo: use reflection to retrieve these strategies.
            return new ITestFrameworkStrategy[]
            {
                new MsTestStrategy(),
                new XunitStrategy()
            };
        }

        private static void InitializeTestAssemblyPaths()
        {
            _testAssemblyPaths = _fileManager.GetTestAssemblyPaths(_inputOptions.Directory, _inputOptions.MinimatchPatterns);
        }

        private static void InitializeAccesses()
        {
            _commandLineAccess = CreateCommandLineAccess(_isLocal, _messages, _azureDevOpsColors);
            _fileAccess = CreateFileAccess(_testFrameWorkStrategy);

            var httpClients = RetrieveHttpClients(CreateVssConnection());
            ValidateDevOpsCredentials(httpClients.TestManagementHttpClient);
            _devOpsAccess = new AzureDevOpsAccess(httpClients, _messages, _commandLineAccess, _inputOptions, _counter);
        }

        private static void InitializeManagers()
        {
            _outputManager = new OutputManager(_messages, _commandLineAccess, _counter);
            _fileManager = new FileManager(_messages, _fileAccess, _commandLineAccess);
            _devOpsManager = new AzureDevOpsManager(_messages, _outputManager, _devOpsAccess, _counter);
        }

        private static AzureDevOpsHttpClients RetrieveHttpClients(VssConnection connection)
        {
            _commandLineAccess.WriteToConsole(_messages.Stages.HttpClient.Status, _messages.Types.Stage);

            var httpClients = new AzureDevOpsHttpClients();

            try
            {
                httpClients.TestManagementHttpClient = connection.GetClient<TestManagementHttpClient>();
                httpClients.WorkItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            }
            catch (Exception e)
            {
                var innerException = e.InnerException ?? e;
                var innerExceptionType = innerException.GetType();
                var message = innerException.Message;

                if (innerExceptionType == typeof(VssServiceResponseException))
                {
                    message = string.Format(_messages.Stages.HttpClient.FailureResourceNotFound, _inputOptions.CollectionUri);
                }
                else if (innerExceptionType == typeof(VssServiceException))
                {
                    message = string.Format(_messages.Stages.HttpClient.FailureUserNotAuthorized, _inputOptions.CollectionUri);
                }

                _commandLineAccess.WriteToConsole(message, _messages.Types.Error);

                throw innerException;
            }

            _commandLineAccess.WriteToConsole(_messages.Stages.HttpClient.Success, _messages.Types.Success);
            return httpClients;
        }

        private static void ValidateDevOpsCredentials(TestManagementHttpClient testManagementHttpClient)
        {
            _commandLineAccess.WriteToConsole(_messages.Stages.DevOpsCredentials.Status, _messages.Types.Stage);

            TestPlan testPlan;
            try
            {
                testPlan = testManagementHttpClient.GetPlansAsync(_inputOptions.ProjectName).Result.Single(x => x.Id.Equals(_inputOptions.TestPlanId));
            }
            catch (Exception e)
            {
                var innerException = e.InnerException ?? e;
                var innerExceptionType = innerException.GetType();
                var message = innerException.Message;

                if (innerExceptionType == typeof(VssUnauthorizedException))
                {
                    message = string.Format(_messages.Stages.DevOpsCredentials.FailureUserNotAuthorized, _inputOptions.CollectionUri);
                }
                else if (innerExceptionType == typeof(ProjectDoesNotExistWithNameException))
                {
                    message = string.Format(_messages.Stages.DevOpsCredentials.FailureNonExistingProject, _inputOptions.ProjectName);
                }
                else if (innerExceptionType == typeof(InvalidOperationException) && e.Message.Equals(SequenceContainsNoMatchingElementName))
                {
                    message = string.Format(_messages.Stages.DevOpsCredentials.FailureNonExistingTestPlan, _inputOptions.TestPlanId);
                }

                _commandLineAccess.WriteToConsole(message, _messages.Types.Error);

                throw innerException;
            }

            try
            {
                _ = testManagementHttpClient.GetTestSuitesForPlanAsync(_inputOptions.ProjectName, testPlan.Id).Result.Single(x => x.Id.Equals(_inputOptions.TestSuiteId));
            }
            catch (Exception e)
            {
                var innerException = e.InnerException ?? e;
                var innerExceptionType = innerException.GetType();
                var message = innerException.Message;

                if (innerExceptionType == typeof(VssUnauthorizedException))
                {
                    message = string.Format(_messages.Stages.DevOpsCredentials.FailureUserNotAuthorized, _inputOptions.CollectionUri);
                }
                else if (innerExceptionType == typeof(InvalidOperationException) && e.Message.Equals(SequenceContainsNoMatchingElementName))
                {
                    message = string.Format(_messages.Stages.DevOpsCredentials.FailureNonExistingTestSuite, _inputOptions.TestSuiteId);
                }

                _commandLineAccess.WriteToConsole(message, _messages.Types.Error);

                throw innerException;
            }

            _commandLineAccess.WriteToConsole(_messages.Stages.DevOpsCredentials.Success, _messages.Types.Success);
        }

        private static CommandLineAccess CreateCommandLineAccess(bool isLocal, Messages messages, AzureDevOpsColors azureDevOpsColors)
        {
            return new CommandLineAccess(isLocal, messages, azureDevOpsColors);
        }

        private static FileAccess CreateFileAccess(ITestFrameworkStrategy testFrameWorkStrategy)
        {
            return new FileAccess(new AssemblyHelper(), testFrameWorkStrategy);
        }

        private static VssConnection CreateVssConnection()
        {
            return new VssConnection(new Uri(_inputOptions.CollectionUri), new VssBasicCredential(string.Empty, _inputOptions.PersonalAccessToken));
        }

        private static void WriteTraceToConsole(Exception e)
        {
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("STACKTRACE: ");
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.TraceError(e.ToString());
            Trace.Close();
        }

        private static void PromptUserToCloseWindow()
        {
            Console.ResetColor();
            Console.Write(PressAnyKeyToCloseWindowName);
            Console.ReadKey();
        }
    }
}
