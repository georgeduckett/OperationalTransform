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
            var siteState = new SiteState(state);
            Assert.AreEqual(state, siteState.CurrentState);
        }
        [TestMethod]
        public void SiteState_ctor_InitialStateHasNoAppliedTransforms()
        {
            var state = "1234";
            var siteState = new SiteState(state);
            Assert.AreEqual(0, siteState.AppliedOperations.Count);
        }
        [TestMethod]
        public void SiteState_ApplyTransform_TransformGetsApplied()
        {
            var siteState = new SiteState("1234");
            var transform = new InsertOperation(1, 1, 0, 'a');
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
            var localSiteState = new SiteState("12345");
            var remoteSiteState = new SiteState("12345");
            var localTransform = new AppliedOperation(new InsertOperation(1, 0, 0, 'x'), localSiteState);
            var remoteTransform = new AppliedOperation(new InsertOperation(2, 0, 2, 'Y'), remoteSiteState);

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
            var localSiteState = new SiteState("ABCDE");
            var remoteSiteState = new SiteState("ABCDE");
            var LocalUser = 1U;
            var RemoteUser = 2U;
            var localTransform = new AppliedOperation(new InsertOperation(LocalUser, 0, 1, '1'), localSiteState);
            var localTransform2 = new AppliedOperation(new InsertOperation(LocalUser, 1, 2, '2'), localTransform.Operation.Id);
            var remoteTransform = new AppliedOperation(DeleteOperation.CreateFromState(RemoteUser, 0, 2, remoteSiteState.CurrentState), remoteSiteState);
            var remoteTransform2 = new AppliedOperation(DeleteOperation.CreateFromState(RemoteUser, 1, 2, remoteSiteState.CurrentState), remoteTransform.Operation.Id);

            localSiteState.ApplyTransform(localTransform);
            localSiteState.ApplyTransform(localTransform2);
            localSiteState.ApplyTransform(remoteTransform);
            localSiteState.ApplyTransform(remoteTransform2);

            Assert.AreEqual("A12BE", localSiteState.CurrentState);
        }
    }
}