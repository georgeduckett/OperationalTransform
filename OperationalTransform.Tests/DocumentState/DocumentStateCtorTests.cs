using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;

namespace OperationalTransform.Tests
{
    [TestClass]
    public class DocumentStateCtorTests
    {
        [TestMethod]
        public void DocumentState_ctor_InitialStateCorrectlySet()
        {
            var state = "1234";
            var DocumentState = new DocumentState(1, state);
            Assert.AreEqual(state, DocumentState.CurrentState);
        }
        [TestMethod]
        public void DocumentState_ctor_InitialStateHasNoAppliedTransforms()
        {
            var state = "1234";
            var DocumentState = new DocumentState(1, state);
            Assert.AreEqual(0, DocumentState.AppliedOperations.Count);
        }
    }
}
