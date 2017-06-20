using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class IdentityOperationTests
    {
        [TestMethod]
        public void IdentityOperation_ApplyTransform_MaintainsState()
        {
            var state = new DocumentState(1, "123456789");

            Assert.AreEqual(state.CurrentState, new IdentityOperation(state).ApplyTransform(state.CurrentState));
        }
        [TestMethod]
        public void IdentityOperation_CreateInverse_InverseMaintainsState()
        {
            var state = new DocumentState(1, "123456789");
            Assert.AreEqual(state.CurrentState, new IdentityOperation(state).CreateInverse(state).ApplyTransform(state.CurrentState));
        }
    }
}
