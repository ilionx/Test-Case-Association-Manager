using AssociateTestsToTestCases;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Manager.File;
using AssociateTestsToTestCases.Access.Output;

namespace Test.Unit.Manager.File
{
    public class FileManagerFactory
    {
        private readonly Messages _messages;
        private readonly IFileAccess _fileAccess;
        private readonly IOutputAccess _outputAccess;

        public FileManagerFactory(IFileAccess fileAccess, IOutputAccess outputAccess)
        {
            _fileAccess = fileAccess;
            _outputAccess = outputAccess;
            _messages = new Messages();
        }

        public IFileManager Create()
        {
            return new FileManager(_messages, _fileAccess, _outputAccess);
        }
    }
}
