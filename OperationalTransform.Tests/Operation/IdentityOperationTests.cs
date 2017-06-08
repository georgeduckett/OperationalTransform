using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class IdentityOperationTests
    {
        [TestMethod]
        public void IdentityOperation_ApplyTransform_MaintainsState()
        {
            var state = "1234";

            Assert.AreEqual(state, new IdentityOperation(1, 1).ApplyTransform(state));
        }
        [TestMethod]
        public void IdentityOperation_CreateInverse_InverseMaintainsState()
        {
            var state = "1234";
            Assert.AreEqual(state, new IdentityOperation(1, 1).CreateInverse().ApplyTransform(state));
        }
    }
}
