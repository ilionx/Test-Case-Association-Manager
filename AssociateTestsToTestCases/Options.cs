using CommandLine;

namespace AssociateTestsToTestCases
{
    public class Options
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

        [Option('v', "validationonly", Required = false, Default = false, HelpText = "Indicates if you only want to validate the changes without saving the test cases.")]
        public bool ValidationOnly { get; set; }

        [Option('l', "verboselogging", Required = false, Default = false, HelpText = "When Verbose logging is turned on it also outputs the successful matchings and the fixes next to the warnings/errors.")]
        public bool VerboseLogging { get; set; }

        [Option('x', "debugmode", Required = false, Default = false, HelpText = "When Debug mode is turned on any exception thrown by the tool will be printed to the console. Use this only in case of issues.")]
        public bool DebugMode { get; set; }
    }
}
