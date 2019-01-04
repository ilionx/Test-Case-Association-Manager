//using AssociateTestsToTestCases.Message;
//using AssociateTestsToTestCases.Access.DevOps;
//using AssociateTestsToTestCases.Access.Output;

//namespace Test.Unit.Access.DevOps
//{
//    public class DevOpsAccessFactory
//    {
//        private readonly Messages _messages;
//        private readonly bool _verboseLogging;
//        private readonly IOutputAccess _outputAccess;
//        private readonly AssociateTestsToTestCases.Counter.Counter _counter;

//        public DevOpsAccessFactory(Messages messages, IOutputAccess outputAccess, bool verboseLogging, AssociateTestsToTestCases.Counter.Counter counter)
//        {
//            _messages = messages;
//            _outputAccess = outputAccess;
//            _verboseLogging = verboseLogging;
//            _counter = counter;
//        }

//        public IDevOpsAccess Create()
//        {
//            return new AzureDevOpsAccess(_messages, _outputAccess, _verboseLogging, _counter);
//        }
//    }
//}
