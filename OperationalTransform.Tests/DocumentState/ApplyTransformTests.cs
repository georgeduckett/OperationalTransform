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
    public class DocumentStateTests
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
        [TestMethod]
        public void DocumentState_ApplyTransform_TransformGetsApplied()
        {
            var DocumentState = new DocumentState(1, "1234");
            var transform = new InsertOperation(DocumentState, 0, 'a');
            DocumentState.ApplyTransform(new AppliedOperation(transform));
            Assert.AreEqual(DocumentState.AppliedOperations.SingleOrDefault().Value, transform);
            Assert.AreEqual("a1234", DocumentState.CurrentState);
        }
        /// <summary>
        /// Tests http://www3.ntu.edu.sg/home/czsun/projects/otfaq/#req_consistency_maintenance - Convergence
        /// </summary>
        [TestMethod]
        public void DocumentState_ApplyTransform_Convergence()
        {
            var localDocumentState = new DocumentState(1, "12345");
            var remoteDocumentState = new DocumentState(2, "12345");
            var localTransform = new AppliedOperation(new InsertOperation(localDocumentState, 0, 'x'), localDocumentState);
            var remoteTransform = new AppliedOperation(new InsertOperation(remoteDocumentState, 2, 'Y'), remoteDocumentState);

            localDocumentState.ApplyTransform(localTransform);
            localDocumentState.ApplyTransform(remoteTransform);

            remoteDocumentState.ApplyTransform(remoteTransform);
            remoteDocumentState.ApplyTransform(localTransform);

            Assert.AreEqual(localDocumentState.CurrentState, remoteDocumentState.CurrentState);
        }
        /// <summary>
        /// Tests http://www3.ntu.edu.sg/home/czsun/projects/otfaq/#req_consistency_maintenance - Intention Preservation
        /// </summary>
        [TestMethod]
        public void DocumentState_ApplyTransform_IntentionPreservation()
        {
            var localDocumentState = new DocumentState(1, "ABCDE");
            var remoteDocumentState = new DocumentState(2, "ABCDE");
            var localTransform = new AppliedOperation(new InsertOperation(localDocumentState, 1, '1'), localDocumentState);
            var localTransform2 = new AppliedOperation(new InsertOperation(localDocumentState, 2, '2'), new[] { localTransform.Operation.Id });
            var remoteTransform = new AppliedOperation(new DeleteOperation(remoteDocumentState, 2), remoteDocumentState);
            var remoteTransform2 = new AppliedOperation(new DeleteOperation(remoteDocumentState, 2), new[] { remoteTransform.Operation.Id });

            localDocumentState.ApplyTransform(localTransform);
            localDocumentState.ApplyTransform(localTransform2);
            localDocumentState.ApplyTransform(remoteTransform);
            localDocumentState.ApplyTransform(remoteTransform2);

            Assert.AreEqual("A12BE", localDocumentState.CurrentState);
        }
    }
}