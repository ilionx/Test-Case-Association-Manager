using System;
using System.Linq;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Message.Type;

namespace AssociateTestsToTestCases.Access.Output
{
    public static class CommandLineAccess
    {
        private const string Indention = "  * ";
        private const string Format = "[{0}]";
        private const string OutputMessage = "{0}{1}{2} {3}{4}{5}";

        public static void WriteToConsole(Messages messages, string message, string messageType = "", string reason = "")
        {
            var indention = message.Equals(string.Empty) | (reason.Length.Equals(0) & new[] { messages.Types.Stage, messages.Types.Success, messages.Types.Error }.Contains(messageType)) ? string.Empty : Indention;

            var messageTypeFormat = messageType.Length.Equals(0) ? string.Empty : string.Format(Format, messageType);
            var spaceMessageType = new string(' ', messages.Types.LongestTypeCount - messageType.Count());

            var reasonOutputFormat = reason.Length.Equals(0) ? string.Empty : string.Format(Format, reason);
            var spaceReason = reason.Length.Equals(0) ? string.Empty : new string(' ', messages.Reasons.LongestReasonCount - reason.Count() + 1);

            Console.ForegroundColor = GetConsoleColor(messages.Types, messageType);
            Console.WriteLine(OutputMessage, indention, messageTypeFormat, spaceMessageType, reasonOutputFormat, spaceReason, message);
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
            }  else if (messageType.Equals(types.Stage))
            {
                consoleColor = ConsoleColor.Gray;
            }

            return consoleColor;
        }
    }
}
