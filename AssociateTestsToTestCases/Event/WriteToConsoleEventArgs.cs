using System;

namespace AssociateTestsToTestCases.Event
{
    public class WriteToConsoleEventArgs : EventArgs
    {
        public string Reason { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
    }
}
