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
            var state1 = new DocumentState(1, "1234");
            var state2 = new DocumentState(2, "1234");

            var op1 = new InsertOperation(state1, 1, 'a'); // TODO: Try this (and others) for each combination of operation (and combination of before/same/after indexes)
            var op2 = new InsertOperation(state2, 1, 'b');

            var op1dash = OperationTransformer.Transform(op1, op2);
            var op2dash = OperationTransformer.Transform(op2, op1);
            var op1undo = op1.CreateInverse();
            var op2undo = op2.CreateInverse();

            var op1undodash = OperationTransformer.Transform(op1undo, op2dash);
            var op1dashundo = op1dash.CreateInverse();

            Assert.IsTrue(op1undodash.IdenticalOperation(op1dashundo));
        }
    }
}
