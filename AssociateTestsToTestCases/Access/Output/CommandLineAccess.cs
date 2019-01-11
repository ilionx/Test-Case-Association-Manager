using System;
using System.Linq;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Message.Type;

namespace AssociateTestsToTestCases.Access.Output
{
    public class CommandLineAccess : IOutputAccess
    {
        private const char SpaceChar = ' ';
        private const int SpaceCharLength = 1;

        private const string ErrorColorOutput = "##vso[task.logissue type=error;]";
        private const string WarningColorOutput = "##vso[task.logissue type=warning;]";

        private readonly bool _isLocal;
        private readonly Messages _messages;
        private readonly AzureDevOpsColors _azureDevOpsColors;

        public CommandLineAccess(bool isLocal, Messages messages, AzureDevOpsColors azureDevOpsColors)
        {
            _isLocal = isLocal;
            _messages = messages;
            _azureDevOpsColors = azureDevOpsColors;
        }

        public void WriteToConsole(string message, string messageType = "", string reason = "")
        {
            var enumerationPointFormat = GetEnumerationPointFormat(message, messageType, reason);
            var messageTypeFormat = GetMessageTypeFormat(messageType);
            var spaceMessageTypeFormat = GetSpaceMessageTypeFormat(messageType);
            var reasonOutputFormat = GetReasonOutputFormat(reason);
            var spaceReasonFormat = GetSpaceReasonFormat(reason);

            var messageOutput = GetMessageOutput(enumerationPointFormat, messageTypeFormat, spaceMessageTypeFormat, reasonOutputFormat, spaceReasonFormat, message, messageType);

            if (_isLocal)
            {
                Console.ForegroundColor = GetConsoleColor(_messages.Types, messageType);
            }

            Console.WriteLine(messageOutput);
        }

        #region Outputformatting

        private string GetEnumerationPointFormat(string message, string messageType, string reason)
        {
            var messageTypesWithNoIndention = new[] { _messages.Types.Stage, _messages.Types.Success, _messages.Types.Error };
            return message.Equals(string.Empty) || (reason.Length.Equals(0) && messageTypesWithNoIndention.Contains(messageType)) ? string.Empty : _messages.EnumerationPoint;
        }

        private string GetMessageTypeFormat(string messageType)
        {
            return messageType.Length.Equals(0) ? string.Empty : string.Format(_messages.Stages.Format, messageType);
        }

        private string GetSpaceMessageTypeFormat(string messageType)
        {
            return new string(SpaceChar, _messages.Types.LongestTypeCount - messageType.Count());
        }

        private string GetReasonOutputFormat(string reason)
        {
            return reason.Length.Equals(0) ? string.Empty : string.Format(_messages.Stages.Format, reason);
        }

        private string GetSpaceReasonFormat(string reason)
        {
            return reason.Length.Equals(0) ? string.Empty : new string(SpaceChar, _messages.Reasons.LongestReasonCount - reason.Count() + 1);
        }

        private string GetMessageOutput(string enumerationPoint, string messageTypeFormat, string spaceMessageType, string reasonOutputFormat, string spaceReason, string message, string messageType)
        {
            var messageOutput = string.Format(_messages.Output, enumerationPoint, messageTypeFormat, spaceMessageType, reasonOutputFormat, spaceReason, message);
            return PrependPipelineColor(messageOutput, messageType);
        }

        private string PrependPipelineColor(string message, string messageType)
        {
            return _isLocal ? message : message.Insert(0, GetAzureDevOpsConsoleColor(messageType));
        }

        #endregion

        #region Outputcolors

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
            } else if (messageType.Equals(types.Stage) || messageType.Equals(string.Empty))
            {
                consoleColor = ConsoleColor.Gray;
            }

            return consoleColor;
        }

        private string GetAzureDevOpsConsoleColor(string messageType)
        {
            var spaceMessageType = string.Empty;
            var colorOutput = _azureDevOpsColors.DefaultColor;
            var azureDevOpsColors = _azureDevOpsColors.LongestTypeCount;

            if (messageType.Equals(_messages.Types.Success))
            {
                colorOutput = _azureDevOpsColors.SuccessColor;
            } else if (messageType.Equals(_messages.Types.Warning))
            {
                colorOutput = WarningColorOutput;
                spaceMessageType = GetAzureDevOpsSpaceMessageTypeFormat(azureDevOpsColors, _azureDevOpsColors.WarningColor.Length);
            } else if (messageType.Equals(_messages.Types.Error))
            {
                colorOutput = ErrorColorOutput;
                spaceMessageType = GetAzureDevOpsSpaceMessageTypeFormat(azureDevOpsColors, _azureDevOpsColors.FailureColor.Length);
            } else if (messageType.Equals(_messages.Types.Failure))
            {
                colorOutput = _azureDevOpsColors.FailureColor;
            } else if (messageType.Equals(_messages.Types.Stage) || messageType.Equals(string.Empty))
            {
                colorOutput = string.Empty;
            }

            if (colorOutput.Length <= _azureDevOpsColors.LongestTypeCount)
            {
                spaceMessageType = GetAzureDevOpsSpaceMessageTypeFormat(azureDevOpsColors, colorOutput.Length);
            }

            colorOutput += spaceMessageType;

            return colorOutput;
        }

        private string GetAzureDevOpsSpaceMessageTypeFormat(int longestAzureDevOpsColorLength, int azureDevOpsColorLength)
        {
            return new string(SpaceChar, SpaceCharLength + longestAzureDevOpsColorLength - azureDevOpsColorLength);
        }

        #endregion
    }
}
