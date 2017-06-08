using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class InsertOperationTests
    {
        [TestMethod]
        public void InsertOperation_ApplyTransform_CorrectlyInserts()
        {
            var state = new DocumentState(1, "123456789");
            Assert.AreEqual("12x3456789", new InsertOperation(state, 2, 'x').ApplyTransform(state.CurrentState));
        }
        [TestMethod]
        public void InsertOperation_CreateInverse_CreatesDeleteOperation()
        {
            var state = new DocumentState(1, "123456789");
            var insert = new InsertOperation(state, 2, 'x');
            Assert.IsInstanceOfType(insert.CreateInverse(), typeof(DeleteOperation));
        }
        [TestMethod]
        public void InsertOperation_CreateInverse_InverseUndoesInsert()
        {
            var state = new DocumentState(1, "123456789");
            var insert = new InsertOperation(state, 2, 'x');
            var insertedstate = insert.ApplyTransform(state.CurrentState);
            Assert.AreEqual(state.CurrentState, insert.CreateInverse().ApplyTransform(insertedstate));
        }
    }
}