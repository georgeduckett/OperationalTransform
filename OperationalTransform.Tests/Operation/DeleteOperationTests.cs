using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class DeleteOperationTests
    {
        [TestMethod]
        public void DeleteOperation_CreateFromState_CreatesCorrectly()
        {
            var state = new DocumentState(1, "1234");
            Assert.AreEqual('2', new DeleteOperation(state, 1).Text);
        }
        [TestMethod]
        public void DeleteOperation_ApplyTransform_CorrectlyDeletes()
        {
            var state = new DocumentState(1, "1234");
            Assert.AreEqual("124", new DeleteOperation(state, 2).ApplyTransform(state.CurrentState));
        }
        [TestMethod]
        public void DeleteOperation_CreateInverse_CreatesInsertOperation()
        {
            var state = new DocumentState(1, "1234");
            var delete = new DeleteOperation(state, 2);
            Assert.IsInstanceOfType(delete.CreateInverse(), typeof(InsertOperation));
        }
        [TestMethod]
        public void DeleteOperation_CreateInverse_InverseUndoesDelete()
        {
            var state = new DocumentState(1, "1234");
            var delete = new DeleteOperation(state, 2);
            var deletedstate = delete.ApplyTransform(state.CurrentState);
            Assert.AreEqual(state.CurrentState, delete.CreateInverse().ApplyTransform(deletedstate));
        }
    }
}
