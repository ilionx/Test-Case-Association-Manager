using System;
using System.Linq;
using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Message.Type;

namespace AssociateTestsToTestCases.Access.Output
{
    public class CommandLineAccess
    {
        private readonly Messages _messages;

        public CommandLineAccess(Messages messages)
        {
            _messages = messages;
        }

        public void WriteToConsole(string message, string messageType = "", string reason = "")
        {
            var indention = message.Equals(string.Empty) | (reason.Length.Equals(0) & new[] { _messages.Types.Stage, _messages.Types.Success, _messages.Types.Error }.Contains(messageType)) ? string.Empty : _messages.Indention;
            var messageTypeFormat = messageType.Length.Equals(0) ? string.Empty : string.Format(_messages.Stages.Format, messageType);
            var spaceMessageType = new string(' ', _messages.Types.LongestTypeCount - messageType.Count());

            var reasonOutputFormat = reason.Length.Equals(0) ? string.Empty : string.Format(_messages.Stages.Format, reason);
            var spaceReason = reason.Length.Equals(0) ? string.Empty : new string(' ', _messages.Reasons.LongestReasonCount - reason.Count() + 1);

            Console.ForegroundColor = GetConsoleColor(_messages.Types, messageType);
            Console.WriteLine(_messages.Output, indention, messageTypeFormat, spaceMessageType, reasonOutputFormat, spaceReason, message);
        }

        private static ConsoleColor GetConsoleColor(TypeMessage types, string messageType)
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

        public void OnWriteToConsole(object sender, WriteToConsoleEventArgs eventArgs)
        {
            WriteToConsole(eventArgs.Message, eventArgs.MessageType, eventArgs.Reason);
        }
    }
}
