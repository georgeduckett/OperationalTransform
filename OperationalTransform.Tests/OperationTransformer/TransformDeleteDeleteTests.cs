using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class TransformDeleteDeleteTests
    {
        [TestMethod]
        public void OperationTransformer_TransformDeleteDelete_LocalBeforeRemote()
        {
            var state = new SiteState(1, "123456789");

            var localOperation = new DeleteOperation(state, 2);
            var remoteOperation = new DeleteOperation(state, 4);

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = state.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);

            Assert.AreEqual("1246789", stateStr);
        }
        [TestMethod]
        public void OperationTransformer_TransformDeleteDelete_LocalAfterRemote()
        {
            var state = new SiteState(1, "123456789");

            var localOperation = new DeleteOperation(state, 4);
            var remoteOperation = new DeleteOperation(state, 2);

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = state.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);

            Assert.AreEqual("1246789", stateStr);
        }
        [TestMethod]
        public void OperationTransformer_TransformDeleteDelete_LocalEqualToRemote()
        {
            var state = new SiteState(1, "123456789");

            var localOperation = new DeleteOperation(state, 2);
            var remoteOperation = new DeleteOperation(state, 2);

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            var stateStr = state.CurrentState;
            stateStr = localOperation.ApplyTransform(stateStr);
            stateStr = transformed.ApplyTransform(stateStr);

            Assert.AreEqual("12456789", stateStr);
        }
    }
}
