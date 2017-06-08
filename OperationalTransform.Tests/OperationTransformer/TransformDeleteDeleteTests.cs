using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class TransformDeleteDeleteTests
    {
        private readonly OperationTransformer Transformer = new OperationTransformer();
        [TestMethod]
        public void OperationTransformer_TransformDeleteDelete_LocalBeforeRemote()
        {
            var state = "123456789";

            var localOperation = DeleteOperation.CreateFromState(1, 1, 2, state);
            var remoteOperation = DeleteOperation.CreateFromState(2, 2, 4, state);

            var transformed = Transformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("1246789", state);
        }
        [TestMethod]
        public void OperationTransformer_TransformDeleteDelete_LocalAfterRemote()
        {
            var state = "123456789";

            var localOperation = DeleteOperation.CreateFromState(1, 1, 4, state);
            var remoteOperation = DeleteOperation.CreateFromState(2, 2, 2, state);

            var transformed = Transformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("1246789", state);
        }
        [TestMethod]
        public void OperationTransformer_TransformDeleteDelete_LocalEqualToRemote()
        {
            var state = "123456789";

            var localOperation = DeleteOperation.CreateFromState(1, 1, 2, state);
            var remoteOperation = DeleteOperation.CreateFromState(2, 2, 2, state);

            var transformed = Transformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12456789", state);
        }
    }
}
