using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class ApplyTransformCaretTests
    {
        [TestMethod]
        public void DocumentState_ApplyTransform_CaretAdjustedForInsertBefore()
        {
            var DocumentState = new DocumentState(1, "1234");
            var transform = new InsertOperation(DocumentState, 0, 'a');
            var caretIndex = 2;
            DocumentState.ApplyTransform(new AppliedOperation(transform), ref caretIndex);
            Assert.AreEqual(3, caretIndex);
        }
        [TestMethod]
        public void DocumentState_ApplyTransform_CaretAdjustedForDeleteBefore()
        {
            var DocumentState = new DocumentState(1, "1234");
            var transform = new DeleteOperation(DocumentState, 0);
            var caretIndex = 2;
            DocumentState.ApplyTransform(new AppliedOperation(transform), ref caretIndex);
            Assert.AreEqual(1, caretIndex);
        }
        [TestMethod]
        public void DocumentState_ApplyTransform_CaretClampedForInsertBefore()
        {
            var DocumentState = new DocumentState(1, "1234");
            var transform = new InsertOperation(DocumentState, 0, 'a');
            var caretIndex = 4;
            DocumentState.ApplyTransform(new AppliedOperation(transform), ref caretIndex);
            Assert.IsTrue(caretIndex <= DocumentState.CurrentState.Length);
        }
        [TestMethod]
        public void DocumentState_ApplyTransform_CaretClampedForDeleteBefore()
        {
            var DocumentState = new DocumentState(1, "1234");
            var transform = new DeleteOperation(DocumentState, 0);
            var caretIndex = 0;
            DocumentState.ApplyTransform(new AppliedOperation(transform), ref caretIndex);
            Assert.IsTrue(caretIndex >= 0);
        }
        [TestMethod]
        public void DocumentState_ApplyTransform_CaretSameForInsertAfter()
        {
            var DocumentState = new DocumentState(1, "1234");
            var transform = new InsertOperation(DocumentState, 3, 'a');
            var caretIndex = 1;
            DocumentState.ApplyTransform(new AppliedOperation(transform), ref caretIndex);
            Assert.AreEqual(1, caretIndex);
        }
        [TestMethod]
        public void DocumentState_ApplyTransform_CaretSameForDeleteAfter()
        {
            var DocumentState = new DocumentState(1, "1234");
            var transform = new DeleteOperation(DocumentState, 3);
            var caretIndex = 1;
            DocumentState.ApplyTransform(new AppliedOperation(transform), ref caretIndex);
            Assert.AreEqual(1, caretIndex);
        }
        [TestMethod]
        public void DocumentState_ApplyTransform_CaretSameForInsertSame()
        {
            var DocumentState = new DocumentState(1, "1234");
            var transform = new InsertOperation(DocumentState, 3, 'a');
            var caretIndex = 3;
            DocumentState.ApplyTransform(new AppliedOperation(transform), ref caretIndex);
            Assert.AreEqual(3, caretIndex);
        }
        [TestMethod]
        public void DocumentState_ApplyTransform_CaretSameForDeleteSame()
        {
            var DocumentState = new DocumentState(1, "1234");
            var transform = new DeleteOperation(DocumentState, 3);
            var caretIndex = 3;
            DocumentState.ApplyTransform(new AppliedOperation(transform), ref caretIndex);
            Assert.AreEqual(3, caretIndex);
        }
    }
}
