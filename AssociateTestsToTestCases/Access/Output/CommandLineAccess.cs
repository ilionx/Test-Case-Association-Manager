using System;
using System.Linq;
using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Message.Type;

namespace AssociateTestsToTestCases.Access.Output
{
    public class CommandLineAccess : IOutputAccess
    {
        private const char SpaceChar = ' ';
        private const string SystemTeamProjectName = "SYSTEM_TeamProject";

        private const string ErrorColorOutput = "##vso[task.logissue type=error;]";
        private const string WarningColorOutput = "##vso[task.logissue type=warning;]";

        private readonly bool _isLocal;
        private readonly Messages _messages;
        private readonly AzureDevOpsColors _azureDevOpsColors;

        public CommandLineAccess(Messages messages, AzureDevOpsColors azureDevOpsColors)
        {
            _messages = messages;
            _azureDevOpsColors = azureDevOpsColors;
            _isLocal = Environment.GetEnvironmentVariable(SystemTeamProjectName) == null;
        }

        public void WriteToConsole(string message, string messageType = "", string reason = "")
        {
            Console.ForegroundColor = GetConsoleColor(_messages.Types, messageType);

            var indention = message.Equals(string.Empty) | (reason.Length.Equals(0) & new[] { _messages.Types.Stage, _messages.Types.Success, _messages.Types.Error }.Contains(messageType)) ? string.Empty : _messages.EnumerationPoint;
            var messageTypeFormat = messageType.Length.Equals(0) ? string.Empty : string.Format(_messages.Stages.Format, messageType);
            var spaceMessageType = new string(SpaceChar, _messages.Types.LongestTypeCount - messageType.Count());

            var reasonOutputFormat = reason.Length.Equals(0) ? string.Empty : string.Format(_messages.Stages.Format, reason);
            var spaceReason = reason.Length.Equals(0) ? string.Empty : new string(SpaceChar, _messages.Reasons.LongestReasonCount - reason.Count() + 1);

            var messageOutput = string.Format(_messages.Output, indention, messageTypeFormat, spaceMessageType, reasonOutputFormat, spaceReason, message);
            if(!_isLocal)
            {
                messageOutput = messageOutput.Insert(0, GetAzureDevOpsConsoleColor(_messages.Types, messageType));
            }

            Console.WriteLine(messageOutput);
        }

        private ConsoleColor GetConsoleColor(TypeMessage types, string messageType)
        {
            var consoleColor = ConsoleColor.DarkCyan;

            if (messageType.Equals(types.Success))
            {
                consoleColor = ConsoleColor.DarkGreen;
            } else if (messageType.Equals(types.Warning))
            {
                consoleColor = ConsoleColor.DarkYellow;
            } else if (messageType.Equals(types.Error) || messageType.Equals(types.Failure))
            {
                consoleColor = ConsoleColor.DarkRed;
            }  else if (messageType.Equals(types.Stage) || messageType.Equals(string.Empty))
            {
                consoleColor = ConsoleColor.Gray;
            }

            return consoleColor;
        }

        private string GetAzureDevOpsConsoleColor(TypeMessage types, string messageType)
        {
            var spaceMessageType = string.Empty;
            var colorOutput = _azureDevOpsColors.DefaultColor;
            var azureDevOpsColors = _azureDevOpsColors.LongestTypeCount + 1;

            if (messageType.Equals(types.Success))
            {
                colorOutput = _azureDevOpsColors.SuccessColor;
            } else if (messageType.Equals(types.Warning))
            {
                colorOutput = WarningColorOutput;
                spaceMessageType = new string(SpaceChar, azureDevOpsColors - _azureDevOpsColors.WarningColor.Length);
            } else if (messageType.Equals(types.Error))
            {
                colorOutput = ErrorColorOutput;
                spaceMessageType = new string(SpaceChar, azureDevOpsColors - _azureDevOpsColors.FailureColor.Length);
            } else if (messageType.Equals(types.Failure))
            {
                colorOutput = _azureDevOpsColors.FailureColor;
            } else if (messageType.Equals(types.Stage) || messageType.Equals(string.Empty))
            {
                colorOutput = string.Empty;
            }

            if (colorOutput.Length <= _azureDevOpsColors.LongestTypeCount)
            {
                spaceMessageType = new string(SpaceChar, azureDevOpsColors - colorOutput.Length);
            }

            colorOutput += spaceMessageType;

            return colorOutput;
        }

        public void OnWriteToConsole(object sender, WriteToConsoleEventArgs eventArgs)
        {
            WriteToConsole(eventArgs.Message, eventArgs.MessageType, eventArgs.Reason);
        }
    }
}
