using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class TransformDeleteInsertTests
    {
        [TestMethod]
        public void OperationTransformer_TransformDeleteInsert_LocalBeforeRemote()
        {
            var localState = new DocumentState(1, "123456789");
            var remoteState = new DocumentState(1, "123456789");

            var localOperation = new DeleteOperation(localState, 2);
            var remoteOperation = new InsertOperation(remoteState, 4, 'a');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = localState.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);

            Assert.AreEqual("124a56789", stateStr);
        }
        [TestMethod]
        public void OperationTransformer_TransformDeleteInsert_LocalAfterRemote()
        {
            var localState = new DocumentState(1, "123456789");
            var remoteState = new DocumentState(1, "123456789");

            var localOperation = new DeleteOperation(localState, 4);
            var remoteOperation = new InsertOperation(remoteState, 2, 'a');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = localState.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);


            Assert.AreEqual("12a346789", stateStr);
        }
        [TestMethod]
        public void OperationTransformer_TransformDeleteInsert_LocalEqualToRemote()
        {
            var localState = new DocumentState(1, "123456789");
            var remoteState = new DocumentState(1, "123456789");

            var localOperation = new DeleteOperation(localState, 2);
            var remoteOperation = new InsertOperation(remoteState, 2, 'a');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = localState.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);


            Assert.AreEqual("12a456789", stateStr);
        }
    }
}
