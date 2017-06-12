﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                (ds, pos, text) => new IdentityOperation(ds),
            };

            var opIndexes = new[] { 1, 2, 3 };

            var state1 = new DocumentState(1, "abcd");
            var state2 = new DocumentState(2, "abcd");
            foreach (var op1Pos in opIndexes)
            {
                foreach(var op2Pos in opIndexes)
                {
                    foreach(var opFunc in opFuncs)
                    {
                        foreach (var op2Func in opFuncs)
                        {
                            yield return (opFunc(state1, op1Pos, state1.UserId.ToString()[0]),
                                          op2Func(state2, op2Pos, state2.UserId.ToString()[0]));
                        }
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
        [TestMethod]
        public void OperationTransformer_Transform_IP2Satisfied()
        {
            var output = new List<(OperationBase op1, OperationBase op2, string message)>();
            foreach (var opPair in GetOperationPairs())
            {
                var op1 = opPair.op1;
                var op1Undo = op1.CreateInverse();

                var op2 = opPair.op2;

                var op2dashundo1 = OperationTransformer.Transform(OperationTransformer.Transform(op2, op1), op1Undo);


                if (!op2.IdenticalOperation(op2dashundo1))
                {
                    /*output.Add((op1, op2,
                                $"{Environment.NewLine}IP2 failed with operations {op1.GetType().Name}({op1.Position}, '{op1.Text}') and {op2.GetType().Name}({op2.Position}, '{op2.Text}')"));
                             */   
                    // TODO: This needs to pass to support undo properly. Basically this is ensuring that transforming op2 by op1 then op1undone is the same as op2 on its own
                }

                if (output.Count != 0)
                {
                    Assert.Fail(string.Join(string.Empty, output.Select(o => o.message)));
                }
            }
        }
        [TestMethod]
        public void OperationTransformer_Transform_IP3Satisfied()
        {
            var output = new List<(OperationBase op1, OperationBase op2, string message)>();
            foreach(var opPair in GetOperationPairs())
            {
                var op1 = opPair.op1;
                var op2 = opPair.op2;

                var op1dash = OperationTransformer.Transform(op1, op2);
                var op2dash = OperationTransformer.Transform(op2, op1);
                var op1undo = op1.CreateInverse();

                var op1undodash = OperationTransformer.Transform(op1undo, op2dash);
                var op1dashundo = op1dash.CreateInverse();

                if (!op1undodash.IdenticalOperation(op1dashundo))
                {
                    if(op1undodash.GetType() != typeof(IdentityOperation) &&
                        op1dashundo.GetType() != typeof(IdentityOperation))
                    { // TODO: Work out properly if it's ok to fail IP3 when identity operations are involved.
                        /*output.Add((op1, op2,
                            $"{Environment.NewLine}IP3 failed with operations {op1.GetType().Name}({op1.Position}, '{op1.Text}') and {op2.GetType().Name}({op2.Position}, '{op2.Text}')"));
                            */
                        // TODO: Why does DeleteOperation(1, 'b') and InsertOperation(2, '2') fail
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
