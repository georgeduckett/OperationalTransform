using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class TransformDeleteInsertTests
    {
        [TestMethod]
        public void OperationTransformer_TransformDeleteInsert_LocalBeforeRemote()
        {
            var state = "123456789";

            var localOperation = DeleteOperation.CreateFromState(1, 1, 2, state);
            var remoteOperation = new InsertOperation(2, 2, 4, 'a');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("124a56789", state);
        }
        [TestMethod]
        public void OperationTransformer_TransformDeleteInsert_LocalAfterRemote()
        {
            var state = "123456789";

            var localOperation = DeleteOperation.CreateFromState(1, 1, 4, state);
            var remoteOperation = new InsertOperation(2, 2, 2, 'a');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12a346789", state);
        }
        [TestMethod]
        public void OperationTransformer_TransformDeleteInsert_LocalEqualToRemote()
        {
            var state = "123456789";

            var localOperation = DeleteOperation.CreateFromState(1, 1, 2, state);
            var remoteOperation = new InsertOperation(2, 2, 2, 'a');

            var transformed = OperationTransformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12a456789", state);
        }
    }
}
