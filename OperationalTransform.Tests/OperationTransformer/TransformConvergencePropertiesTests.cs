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
            Assert.Inconclusive("Not yet checked");
        }
    }
}
