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
        private IEnumerable<(OperationBase op1, OperationBase op2)> GetOperationPairs()
        {
            var opFuncs = new Func<DocumentState, int, char, OperationBase>[] {
                (ds, pos, text) => new InsertOperation(ds, pos, text),
                (ds, pos, text) => new DeleteOperation(ds, pos),
                //(ds, pos, text) => new IdentityOperation(ds),
            };

            var opIndexes = new[] { 1, 2, 3 };

            var state1 = new DocumentState(1, "abcd");
            var state2 = new DocumentState(2, "abcd");
            foreach (var op1Pos in opIndexes)
            {
                foreach (var opFunc in opFuncs)
                {
                    foreach (var op2Func in opFuncs)
                    {
                        yield return (opFunc(state1, op1Pos, state1.UserId.ToString()[0]),
                                        op2Func(state2, 2, state2.UserId.ToString()[0]));
                    }
                }
            }
        }
        [TestMethod]
        public void OperationTransformer_Transform_IP1Satisfied()
        {
            new InsertOperationTests().InsertOperation_CreateInverse_InverseUndoesInsert();
            new DeleteOperationTests().DeleteOperation_CreateInverse_InverseUndoesDelete();
            new IdentityOperationTests().IdentityOperation_CreateInverse_InverseMaintainsState();
        }
        [TestMethod][Ignore] // TODO: Get IP2 succeeding (if needed)
        public void OperationTransformer_Transform_IP2Satisfied()
        {
            var output = new List<(OperationBase op1, OperationBase op2, string message)>();
            foreach (var opPair in GetOperationPairs())
            {
                var opx = opPair.op1;
                var op = opPair.op2;

                OperationBase opUndo = op.CreateInverse();
                if (!opx.IdenticalOperation(OperationTransformer.Transform(opx, OperationTransformer.Transform(op, opUndo))))
                {
                    output.Add((opx, op,
                                $"{Environment.NewLine}IP2 failed with opx = {opx} and op = {op}"));   
                    // TODO: This needs to pass to support undo properly. Basically this is ensuring that transforming op2 by op1 then op1undone is the same as op2 on its own
                }
            }

            if (output.Count != 0)
            {
                Assert.Fail(string.Join(string.Empty, output.Select(o => o.message)));
            }
        }
        [TestMethod][Ignore]
        public void OperationTransformer_Transform_IP3Satisfied()
        {
            var output = new List<(OperationBase op1, OperationBase op2, string message)>();
            foreach(var opPair in GetOperationPairs())
            {
                var op1 = opPair.op1;
                var op2 = opPair.op2;
                // TOOD: Create this in terms of document states?
                var op1dash = OperationTransformer.Transform(op1, op2);
                var op2dash = OperationTransformer.Transform(op2, op1);
                var op1undo = op1.CreateInverse();

                var op1undodash = OperationTransformer.Transform(op1undo, op2dash);
                var op1dashundo = op1dash.CreateInverse();

                if (!op1undodash.IdenticalOperation(op1dashundo))
                {
                    if(op1undodash.GetType() != typeof(IdentityOperation) &&
                        op1dashundo.GetType() != typeof(IdentityOperation))
                    {
                       output.Add((op1, op2,
                            $"{Environment.NewLine}IP3 failed with operations {op1.GetType().Name}({op1.Position}, '{op1.Text}') and {op2.GetType().Name}({op2.Position}, '{op2.Text}')"));
                    }
                }
            }

            if(output.Count != 0)
            {
                Assert.Fail(string.Join(string.Empty, output.Select(o => o.message)));
            }
        }
    }
}
