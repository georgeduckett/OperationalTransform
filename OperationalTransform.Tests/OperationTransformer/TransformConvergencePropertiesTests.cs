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
    /// Tests the transform function satisfies the Convergence Properties as per https://en.wikipedia.org/wiki/Operational_transformation#Convergence_properties
    /// </summary>
    [TestClass]
    public class TransformConvergencePropertiesTests
    {
        [TestMethod]
        public void OperationTransformer_Transform_CP1TP1Satisfied()
        {
            var site1Str = "abcd";
            var site2Str = "abcd";
            var site1 = new SiteState(1, site1Str);
            var site2 = new SiteState(2, site2Str);

            var op1 = new InsertOperation(site1, 0, '1');
            var op2 = new InsertOperation(site2, 0, '2');

            site1Str = op2.ApplyTransform(site1Str);
            site1Str = OperationTransformer.Transform(op1, op2).ApplyTransform(site1Str);

            site2Str = op1.ApplyTransform(site2Str);
            site2Str = OperationTransformer.Transform(op2, op1).ApplyTransform(site2Str);

            Assert.AreEqual(site1Str, site2Str);
        }
        [TestMethod]
        public void OperationTransformer_Transform_CP2TP2Satisfied()
        {
            var site1Str = "abcd";
            var site2Str = "abcd";
            var site3Str = "abcd";
            var site1 = new SiteState(1, site1Str);
            var site2 = new SiteState(2, site2Str);
            var site3 = new SiteState(3, site3Str);

            var op1 = new InsertOperation(site1, 0, '1');
            var op2 = new InsertOperation(site2, 1, '2');
            var op3 = new InsertOperation(site3, 2, '3');

            var op2dash = OperationTransformer.Transform(op2, op1);
            var op3dashV1 = OperationTransformer.Transform(OperationTransformer.Transform(op3, op1), op2dash);

            var op1dash = OperationTransformer.Transform(op1, op2);
            var op3dashV2 = OperationTransformer.Transform(OperationTransformer.Transform(op3, op2), op1dash);

            Assert.IsTrue(op3dashV1.IdenticalOperation(op3dashV2));
        }
    }
}
