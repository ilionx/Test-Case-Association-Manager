using System;
using System.Linq;
using AssociateTestsToTestCases.Message;

namespace AssociateTestsToTestCases.Access.Output
{
    public static class CommandLineAccess
    {
        public static void WriteToConsole(Messages messages, string message, string messageType = "", string reason = "")
        {
            var reasonOutputFormat = reason.Length.Equals(0) ? string.Empty : $"[{reason}]";
            var messageTypeFormat = messageType.Length.Equals(0) ? string.Empty : $"[{ messageType}]";

            var spaceMessageType = new string(' ', messages.Types.LongestTypeCount - messageType.Count());
            var spaceReason = reason.Length.Equals(0) ? string.Empty : new string(' ', messages.Reasons.LongestReasonCount - reason.Count() + 1);

            var outputMessage = $"{messageTypeFormat}{spaceMessageType} {reasonOutputFormat}{spaceReason}{message}";
            Console.WriteLine(outputMessage);
        }
    }
}
