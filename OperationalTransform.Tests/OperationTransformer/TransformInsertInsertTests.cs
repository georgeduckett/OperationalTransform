using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class TransformInsertInsertTests
    {
        [TestMethod]
        public void OperationTransformer_TransformInsertInsert_LocalBeforeRemote()
        {
            var state = "123456789";

            var localOperation = new InsertOperation(1, 1, 2, 'a');
            var remoteOperation = new InsertOperation(2, 2, 4, 'b');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12a34b56789", state);
        }
        [TestMethod]
        public void OperationTransformer_TransformInsertInsert_LocalAfterRemote()
        {
            var state = "123456789";

            var localOperation = new InsertOperation(1, 1, 4, 'a');
            var remoteOperation = new InsertOperation(2, 2, 2, 'b');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12b34a56789", state);
        }
        [TestMethod]
        public void OperationTransformer_TransformInsertInsert_LocalEqualToRemote()
        {
            var state = "123456789";

            var localOperation = new InsertOperation(1, 1, 2, 'a');
            var remoteOperation = new InsertOperation(2, 2, 2, 'b');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12ba3456789", state);
        }
    }
}
