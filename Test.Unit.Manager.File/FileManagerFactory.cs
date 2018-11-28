using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Manager.File;

namespace Test.Unit.Manager.File
{
    public class FileManagerFactory
    {
        private readonly Messages _messages;
        private readonly IFileAccess _fileAccess;
        private readonly IWriteToConsoleEventLogger _writeToConsoleEventLogger;

        public FileManagerFactory(IFileAccess fileAccess, IWriteToConsoleEventLogger writeToConsoleEventLogger, Messages messages = null)
        {
            _fileAccess = fileAccess;
            _writeToConsoleEventLogger = writeToConsoleEventLogger;
            _messages = messages ?? new Messages();
        }

        public IFileManager Create()
        {
            return new FileManager(_messages, _fileAccess, _writeToConsoleEventLogger);
        }
    }
}
