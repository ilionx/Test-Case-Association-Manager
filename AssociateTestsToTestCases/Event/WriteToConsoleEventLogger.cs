using System;

namespace AssociateTestsToTestCases.Event
{
    public class WriteToConsoleEventLogger : IWriteToConsoleEventLogger
    {
        public event EventHandler<WriteToConsoleEventArgs> WriteToConsole;

        public void Write(string message, string messageType = "", string reason = "")
        {
            OnWriteToConsole(message, messageType, reason);
        }

        protected virtual void OnWriteToConsole(string messages, string messageType, string reason)
        {
            WriteToConsole?.Invoke(this, new WriteToConsoleEventArgs() { Message = messages, MessageType = messageType, Reason = reason });
        }
    }
}
