using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalTransform.Operations;
using OperationalTransform.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Tests
{
    /// <summary>
    /// Tests the transform function satisfies the Inverse Properties as per https://en.wikipedia.org/wiki/Operational_transformation#Inverse_properties
    /// </summary>
    [TestClass]
    public class TransformInversePropertiesTests
    {
        [TestMethod]
        public void OperationTransformer_Transform_IP1Satisfied()
        {
            new InsertOperationTests().InsertOperation_CreateInverse_InverseUndoesInsert();
            new DeleteOperationTests().DeleteOperation_CreateInverse_InverseUndoesDelete();
            new IdentityOperationTests().IdentityOperation_CreateInverse_InverseMaintainsState();
        }
        [TestMethod]
        public void OperationTransformer_Transform_IP2Satisfied()
        {
            var state = new DocumentState(1, "1234");
            var op = new InsertOperation(state, 1, 'a');
            var opUndo = op.CreateInverse();

            var opx = new InsertOperation(state, 3, 'b');

            var transformedopx = OperationTransformer.Transform(OperationTransformer.Transform(opx, op), opUndo);

            Assert.IsTrue(opx.IdenticalOperation(transformedopx));
        }
        [TestMethod]
        public void OperationTransformer_Transform_IP3Satisfied()
        {
            Assert.Inconclusive();
        }
    }
}
