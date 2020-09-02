using System;
using System.Linq;
using CommandLine;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Validation;
using AssociateTestsToTestCases.Access.Output;

namespace AssociateTestsToTestCases.Parsing
{
    public class CommandLineArgumentsParser
    {
        private readonly IOutputAccess _commandLineAccess;
        private readonly Messages _messages;

        public CommandLineArgumentsParser(IOutputAccess commandlineAccess, Messages messages)
        {
            _commandLineAccess = commandlineAccess;
            _messages = messages;
        }

        public InputOptions Parse(string[] args)
        {
            _commandLineAccess.WriteToConsole(_messages.Stages.Argument.Status, _messages.Types.Stage);

            var _inputOptions = new InputOptions();
            using (var parser = new Parser(config => config.HelpWriter = null))
            {
                parser.ParseArguments<Options>(args)
                    .WithParsed(o =>
                    {
                        _inputOptions.TestType = o.TestType;
                        _inputOptions.DebugMode = o.DebugMode;
                        _inputOptions.Directory = o.Directory;
                        _inputOptions.ProjectName = o.ProjectName;
                        _inputOptions.TestPlanId = int.Parse(o.TestPlanId);
                        _inputOptions.TestSuiteId = int.Parse(o.TestSuiteId);
                        _inputOptions.CollectionUri = o.CollectionUri;
                        _inputOptions.ValidationOnly = o.ValidationOnly;
                        _inputOptions.VerboseLogging = o.VerboseLogging;
                        _inputOptions.PersonalAccessToken = o.PersonalAccessToken;
                        _inputOptions.MinimatchPatterns = o.MinimatchPatterns.Split(';').Select(s => s.ToLowerInvariant()).ToArray();
                        _inputOptions.TestFrameworkType = o.TestFrameworkType;
                    });
            }

            ValidateInputOptions(_inputOptions);

            _commandLineAccess.WriteToConsole(_messages.Stages.Argument.Success, _messages.Types.Success);
            return _inputOptions;
        }

        private void ValidateInputOptions(InputOptions _inputOptions)
        {
            var validationResults = new InputOptionsValidator().Validate(_inputOptions);
            if (validationResults.IsValid)
            {
                return;
            }

            foreach (var error in validationResults.Errors)
            {
                _commandLineAccess.WriteToConsole(error.ErrorMessage, _messages.Types.Failure);
            }

            _commandLineAccess.WriteToConsole(_messages.Stages.Argument.Failure, _messages.Types.Error);

            throw new InvalidOperationException(_messages.Stages.Argument.Failure);
        }
    }
}
