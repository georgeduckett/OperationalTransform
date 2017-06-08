using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class InsertOperationTests
    {
        [TestMethod]
        public void InsertOperation_ApplyTransform_CorrectlyInserts()
        {
            var state = "1234";
            Assert.AreEqual("12x34", new InsertOperation(1, 1, 2, 'x').ApplyTransform(state));
        }
        [TestMethod]
        public void InsertOperation_CreateInverse_CreatesDeleteOperation()
        {
            var insert = new InsertOperation(1, 1, 2, 'x');
            Assert.IsInstanceOfType(insert.CreateInverse(), typeof(DeleteOperation));
        }
        [TestMethod]
        public void InsertOperation_CreateInverse_InverseUndoesInsert()
        {
            var state = "1234";
            var insert = new InsertOperation(1, 1, 2, 'x');
            var insertedstate = insert.ApplyTransform(state);
            Assert.AreEqual(state, insert.CreateInverse().ApplyTransform(insertedstate));
        }
    }
}