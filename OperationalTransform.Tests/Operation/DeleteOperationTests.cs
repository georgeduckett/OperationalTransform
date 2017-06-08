using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class OperationTests
    {
        [TestMethod]
        public void DeleteOperation_CreateFromState_CreatesCorrectly()
        {
            var state = "1234";
            Assert.AreEqual('2', DeleteOperation.CreateFromState(1, 1, 1, state).Text);
        }
        [TestMethod]
        public void DeleteOperation_ApplyTransform_CorrectlyDeletes()
        {
            var state = "1234";
            Assert.AreEqual("124", new DeleteOperation(1, 1, 2, '3').ApplyTransform(state));
        }
        [TestMethod]
        public void DeleteOperation_CreateInverse_CreatesInsertOperation()
        {
            var delete = new DeleteOperation(1, 1, 2, '3');
            Assert.IsInstanceOfType(delete.CreateInverse(), typeof(InsertOperation));
        }
        [TestMethod]
        public void DeleteOperation_CreateInverse_InverseUndoesDelete()
        {
            var state = "1234";
            var delete = new DeleteOperation(1, 1, 2, '3');
            var deletedstate = delete.ApplyTransform(state);
            Assert.AreEqual(state, delete.CreateInverse().ApplyTransform(deletedstate));
        }
    }
}
