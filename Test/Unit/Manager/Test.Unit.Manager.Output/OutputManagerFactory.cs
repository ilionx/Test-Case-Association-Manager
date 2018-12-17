using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Manager.Output;

namespace Test.Unit.Manager.Output
{
    public class OutputManagerFactory
    {
        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;

        public OutputManagerFactory(Messages messages, IOutputAccess outputAccess )
        {
            _messages = messages;
            _outputAccess = outputAccess;
        }

        public IOutputManager Create()
        {
            return new OutputManager(_messages, _outputAccess);
        }
    }
}
