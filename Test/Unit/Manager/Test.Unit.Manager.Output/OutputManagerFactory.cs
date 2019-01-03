using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Manager.Output;

namespace Test.Unit.Manager.Output
{
    public class OutputManagerFactory
    {
        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;
        private readonly Counter _counter;

        public OutputManagerFactory(Messages messages, IOutputAccess outputAccess, Counter counter = null)
        {
            _messages = messages;
            _outputAccess = outputAccess;
            _counter = counter ?? new Counter();
        }

        public IOutputManager Create()
        {
            return new OutputManager(_messages, _outputAccess, _counter);
        }
    }
}
