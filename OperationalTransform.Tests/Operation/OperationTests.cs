using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class OperationTests
    {
        [TestMethod]
        public void IdentityOperation_MaintainsState()
        {
            var state = "1234";

            Assert.AreEqual(state, new IdentityOperation(1).ApplyTransform(state));
        }
        [TestMethod]
        public void InsertOperation_CorrectlyInserts()
        {
            var state = "1234";
            Assert.AreEqual("12x34", new InsertOperation(1, 2, 'x').ApplyTransform(state));
        }
        [TestMethod]
        public void InsertOperation_InverseUndoesInsert()
        {
            var state = "1234";
            var insert = new InsertOperation(1, 2, 'x');
            var insertedstate = insert.ApplyTransform(state);
            Assert.AreEqual(state, insert.CreateInverse().ApplyTransform(insertedstate));
        }
        [TestMethod]
        public void DeleteOperation_CorrectlyDeletes()
        {
            var state = "1234";
            Assert.AreEqual("124", new DeleteOperation(1, 2, '3').ApplyTransform(state));
        }
        [TestMethod]
        public void DeleteOperation_InverseUndoesDelete()
        {
            var state = "1234";
            var insert = new DeleteOperation(1, 2, '3');
            var insertedstate = insert.ApplyTransform(state);
            Assert.AreEqual(state, insert.CreateInverse().ApplyTransform(insertedstate));
        }
    }
}
