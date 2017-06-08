using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class TransformInsertDeleteTests
    {
        private readonly OperationTransformer Transformer = new OperationTransformer();
        [TestMethod]
        public void OperationTransformer_TransformInsertDelete_LocalBeforeRemote()
        {
            var state = "123456789";

            var localOperation = new InsertOperation(1, 1, 2, 'a');
            var remoteOperation = DeleteOperation.CreateFromState(2, 2, 4, state);

            var transformed = Transformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12a346789", state);
        }
        [TestMethod]
        public void OperationTransformer_TransformInsertDelete_LocalAfterRemote()
        {
            var state = "123456789";

            var localOperation = new InsertOperation(1, 1, 4, 'a');
            var remoteOperation = DeleteOperation.CreateFromState(2, 2, 2, state);

            var transformed = Transformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("124a56789", state);
        }
        [TestMethod]
        public void OperationTransformer_TransformInsertDelete_LocalEqualToRemote()
        {
            var state = "123456789";

            var localOperation = new InsertOperation(1, 1, 2, 'a');
            var remoteOperation = DeleteOperation.CreateFromState(2, 2, 2, state);

            var transformed = Transformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12a456789", state);
        }
    }
}
