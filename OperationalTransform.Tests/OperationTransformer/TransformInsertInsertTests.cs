using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class TransformInsertInsertTests
    {
        [TestMethod]
        public void OperationTransformer_TransformInsertInsert_LocalBeforeRemote()
        {
            var localState = new SiteState(0, "123456789");
            var remoteState = new SiteState(1, "123456789");

            var localOperation = new InsertOperation(localState, 2, 'a');
            var remoteOperation = new InsertOperation(remoteState, 4, 'b');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = localState.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);

            Assert.AreEqual("12a34b56789", stateStr);
        }
        [TestMethod]
        public void OperationTransformer_TransformInsertInsert_LocalAfterRemote()
        {
            var localState = new SiteState(0, "123456789");
            var remoteState = new SiteState(1, "123456789");

            var localOperation = new InsertOperation(localState, 4, 'a');
            var remoteOperation = new InsertOperation(remoteState, 2, 'b');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = localState.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);

            Assert.AreEqual("12b34a56789", stateStr);
        }
        [TestMethod]
        public void OperationTransformer_TransformInsertInsert_LocalEqualToRemote()
        {
            var localState = new SiteState(0, "123456789");
            var remoteState = new SiteState(1, "123456789");

            var localOperation = new InsertOperation(localState, 2, 'a');
            var remoteOperation = new InsertOperation(remoteState, 2, 'b');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = localState.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);

            Assert.AreEqual("12ba3456789", stateStr);
        }
    }
}
