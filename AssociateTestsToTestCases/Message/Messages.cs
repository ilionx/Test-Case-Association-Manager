﻿using AssociateTestsToTestCases.Message.Type;
using AssociateTestsToTestCases.Message.Stage;
using AssociateTestsToTestCases.Message.Reason;

namespace AssociateTestsToTestCases.Message
{
    public class Messages
    {
        public readonly string EnumerationPoint = "  * ";

        public readonly TypeMessage Types;
        public readonly StageMessages Stages;
        public readonly ReasonMessage Reasons;
        public readonly TestCase.TestCaseMessage TestCases;
        public readonly TestMethod.TestMethodMessage TestMethods;
        public readonly Association.AssociationMessage Associations;

        public Messages()
        {
            Types = new TypeMessage();
            Stages = new StageMessages();
            Reasons = new ReasonMessage();
            Associations = new Association.AssociationMessage();
            TestCases =  new TestCase.TestCaseMessage();
            TestMethods = new TestMethod.TestMethodMessage();
        }
    }
}
