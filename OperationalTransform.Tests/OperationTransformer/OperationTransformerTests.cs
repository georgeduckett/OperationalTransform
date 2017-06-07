﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class OperationTransformerTests
    {
        private readonly OperationTransformer Transformer = new OperationTransformer();
        [TestMethod]
        public void OperationTransformer_InsertInsert_LocalBeforeRemote()
        {
            var state = "123456789";

            var localOperation = new InsertOperation(1, 2, 'a');
            var remoteOperation = new InsertOperation(2, 4, 'b');

            var transformed = Transformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12a34b56789", state);
        }
        [TestMethod]
        public void OperationTransformer_InsertInsert_LocalAfterRemote()
        {
            var state = "123456789";

            var localOperation = new InsertOperation(1, 4, 'a');
            var remoteOperation = new InsertOperation(2, 2, 'b');

            var transformed = Transformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12b34a56789", state);
        }
        [TestMethod]
        public void OperationTransformer_InsertInsert_LocalEqualToRemote()
        {
            var state = "123456789";

            var localOperation = new InsertOperation(1, 2, 'a');
            var remoteOperation = new InsertOperation(2, 2, 'b');

            var transformed = Transformer.Transform(remoteOperation, localOperation);

            state = localOperation.ApplyTransform(state);
            state = transformed.ApplyTransform(state);

            Assert.AreEqual("12ba3456789", state);
        }
    }
}
