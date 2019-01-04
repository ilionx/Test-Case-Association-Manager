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
        private readonly InputOptions _options;

        public FileManagerFactory(IFileAccess fileAccess, IOutputAccess outputAccess, InputOptions options = null, Messages messages = null)
        {
            _fileAccess = fileAccess;
            _outputAccess = outputAccess;
            _messages = messages ?? new Messages();
            _options = options ?? new InputOptions();
        }

        public IFileManager Create()
        {
            return new FileManager(_messages, _fileAccess, _outputAccess, _options);
        }
    }
}
