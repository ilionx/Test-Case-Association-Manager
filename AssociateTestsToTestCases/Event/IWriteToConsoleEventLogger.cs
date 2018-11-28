using System;

namespace AssociateTestsToTestCases.Event
{
    public interface IWriteToConsoleEventLogger
    {
        event EventHandler<WriteToConsoleEventArgs> WriteToConsole;

        void Write(string message, string messageType = "", string reason = "");
    }
}
