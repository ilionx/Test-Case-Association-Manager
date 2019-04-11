using System;
using System.Linq;
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

        private static void Main(string[] args)
        {
            try
            {
                OutputInitMessage();
                InitializeProgram(args);

                _outputManager.WriteToConsole(_messages.Stages.Project.Status, _messages.Types.Stage);

                var projectHasNoTestSetup = _fileManager.TestMethodsPathIsEmpty(_testAssemblyPaths) && _devOpsManager.TestPlanIsEmpty();
                if (projectHasNoTestSetup)
                {
                    _outputManager.WriteToConsole(_messages.Stages.Project.Failure, _messages.Types.Warning);
                }
                else
                {
                    _outputManager.WriteToConsole(_messages.Stages.Project.Success, _messages.Types.Success);

                    _testMethods = _fileManager.GetTestMethods(_testAssemblyPaths);
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
                PromptUserToCloseWindow();
            }
        }

        private static void OutputInitMessage()
        {
            // Todo: logo is not correct yet!
            Console.WriteLine(@"                                                                    
                             ./sss+.                                
                         `:smMMMMMMMmy/`                            
                      .+hMMMMMh+/ohMMMMMdo-                         
                  `/ymMMMMms:`     `:smMMMMNy/`                     
               `odMMMMNh+.``           .+hNMMMMdo.                  
              :NMMMms:    mm               :smMMMM/                 
              mMMN.       Nm                  -MMMN                 
              NMMN   oddddMMdmmmmmmmmh:        NMMM`                
              NMMN   `....Mm`````````dM.       NMMM`                
              NMMN        Md         yM.       NMMM`                
              NMMN        Md         yM.       NMMM`                
              NMMN       `Md         yM.       NMMM`                
              NMMN        Nm:--------dM/---.   NMMM`                
              NMMN        .oyyyyyyyyymMhyyyo   NMMM`                
              dMMM+`                 yM.     .oMMMN                 
              -mMMMMdo-              sN.  :sdMMMMm-                 
              | :smMMMMNh+.           .+hNMMMMNy/ |                 
              |    .+hNMMMMms:    `:smMMMMMdo-    |                 
              |       `:smMMMMNhshMMMMMms:`       |                 
              |           .+hNMMMMMNh+.           |                 
              |               -/+/-`              |                 
    ___       |                 _       __  _     |       ______    
   /   |  ____|____ ____  _____(_)___ _/ /_(_)__  |____  / ____/  __
  / /| | / ___/ ___/ __ \/ ___/ / __ `/ __/ / __ \/ __ \/ __/ | |/_/
 / ___ |(__  |__  ) /_/ / /__/ / /_/ / /_/ / /_/ / / / / /____>  <  
/_/  |_/____/____/\____/\___/_/\__,_/\__/_/\____/_/ /_/_____/_/|_|  
    |     |    |    |    |   |   |    |  |   |     |    |     |      ");
            Console.WriteLine("===================================================================\n");
        }

        private static void InitializeProgram(string[] args)
        {
            _messages = new Messages();
            _counter = new Counter.Counter();
            _isLocal = Environment.GetEnvironmentVariable(SystemTeamProjectName) == null;
            _inputOptions = new CommandLineArgumentsParser(CreateCommandLineAccess(_isLocal, _messages, new AzureDevOpsColors()), _messages).Parse(args);
            _testAssemblyPaths = CreateFileAccess().ListTestAssemblyPaths(_inputOptions.Directory, _inputOptions.MinimatchPatterns);

            InitializeAccesses();
            InitializeManagers();
        }

        private static void InitializeAccesses()
        {
            _commandLineAccess = CreateCommandLineAccess(_isLocal, _messages, new AzureDevOpsColors());
            _fileAccess = CreateFileAccess();

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

            try
            {
                testManagementHttpClient.GetPlansAsync(_inputOptions.ProjectName).Result.Single(x => x.Name.Equals(_inputOptions.TestPlanName));
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
                    message = string.Format(_messages.Stages.DevOpsCredentials.FailureNonExistingTestPlan, _inputOptions.TestPlanName);
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

        private static FileAccess CreateFileAccess()
        {
            return new FileAccess(new AssemblyHelper());
        }

        private static VssConnection CreateVssConnection()
        {
            return new VssConnection(new Uri(_inputOptions.CollectionUri), new VssBasicCredential(string.Empty, _inputOptions.PersonalAccessToken));
        }

        private static void PromptUserToCloseWindow()
        {
            Console.ResetColor();
            Console.Write(PressAnyKeyToCloseWindowName);
            Console.ReadKey();
        }
    }
}
