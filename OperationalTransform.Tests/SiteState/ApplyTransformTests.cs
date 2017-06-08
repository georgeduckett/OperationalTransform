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
    [TestClass]
    public class SiteStateTests
    {
        [TestMethod]
        public void SiteState_ctor_InitialStateCorrectlySet()
        {
            var state = "1234";
            var siteState = new SiteState(1, state);
            Assert.AreEqual(state, siteState.CurrentState);
        }
        [TestMethod]
        public void SiteState_ctor_InitialStateHasNoAppliedTransforms()
        {
            var state = "1234";
            var siteState = new SiteState(1, state);
            Assert.AreEqual(0, siteState.AppliedOperations.Count);
        }
        [TestMethod]
        public void SiteState_ApplyTransform_TransformGetsApplied()
        {
            var siteState = new SiteState(1, "1234");
            var transform = new InsertOperation(siteState, 0, 'a');
            siteState.ApplyTransform(new AppliedOperation(transform));
            Assert.AreEqual(siteState.AppliedOperations.SingleOrDefault().Value, transform);
            Assert.AreEqual("a1234", siteState.CurrentState);
        }
        /// <summary>
        /// Tests http://www3.ntu.edu.sg/home/czsun/projects/otfaq/#req_consistency_maintenance - Convergence
        /// </summary>
        [TestMethod]
        public void SiteState_ApplyTransform_Convergence()
        {
            var localSiteState = new SiteState(1, "12345");
            var remoteSiteState = new SiteState(2, "12345");
            var localTransform = new AppliedOperation(new InsertOperation(localSiteState, 0, 'x'), localSiteState);
            var remoteTransform = new AppliedOperation(new InsertOperation(remoteSiteState, 2, 'Y'), remoteSiteState);

            localSiteState.ApplyTransform(localTransform);
            localSiteState.ApplyTransform(remoteTransform);

            remoteSiteState.ApplyTransform(remoteTransform);
            remoteSiteState.ApplyTransform(localTransform);

            Assert.AreEqual(localSiteState.CurrentState, remoteSiteState.CurrentState);
        }
        /// <summary>
        /// Tests http://www3.ntu.edu.sg/home/czsun/projects/otfaq/#req_consistency_maintenance - Intention Preservation
        /// </summary>
        [TestMethod]
        public void SiteState_ApplyTransform_IntentionPreservation()
        {
            var localSiteState = new SiteState(1, "ABCDE");
            var remoteSiteState = new SiteState(2, "ABCDE");
            var localTransform = new AppliedOperation(new InsertOperation(localSiteState, 1, '1'), localSiteState);
            var localTransform2 = new AppliedOperation(new InsertOperation(localSiteState, 2, '2'), new[] { localTransform.Operation.Id });
            var remoteTransform = new AppliedOperation(new DeleteOperation(remoteSiteState, 2), remoteSiteState);
            var remoteTransform2 = new AppliedOperation(new DeleteOperation(remoteSiteState, 2), new[] { remoteTransform.Operation.Id });

            localSiteState.ApplyTransform(localTransform);
            localSiteState.ApplyTransform(localTransform2);
            localSiteState.ApplyTransform(remoteTransform);
            localSiteState.ApplyTransform(remoteTransform2);

            Assert.AreEqual("A12BE", localSiteState.CurrentState);
        }
    }
}