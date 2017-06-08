using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class TransformInsertDeleteTests
    {
        [TestMethod]
        public void OperationTransformer_TransformInsertDelete_LocalBeforeRemote()
        {
            var state = new DocumentState(1, "123456789");

            var localOperation = new InsertOperation(state, 2, 'a');
            var remoteOperation = new DeleteOperation(state, 4);

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = state.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);


            Assert.AreEqual("12a346789", stateStr);
        }
        [TestMethod]
        public void OperationTransformer_TransformInsertDelete_LocalAfterRemote()
        {
            var state = new DocumentState(1, "123456789");

            var localOperation = new InsertOperation(state, 4, 'a');
            var remoteOperation = new DeleteOperation(state, 2);

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = state.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);


            Assert.AreEqual("124a56789", stateStr);
        }
        [TestMethod]
        public void OperationTransformer_TransformInsertDelete_LocalEqualToRemote()
        {
            var state = new DocumentState(1, "123456789");

            var localOperation = new InsertOperation(state, 2, 'a');
            var remoteOperation = new DeleteOperation(state, 2);

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = state.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);


            Assert.AreEqual("12a456789", stateStr);
        }
    }
}
