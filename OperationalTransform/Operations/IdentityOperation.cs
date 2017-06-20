using OperationalTransform.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    /// <summary>
    /// Represents an operation that does not alter state
    /// </summary>
    public class IdentityOperation : OperationBase
    {
        private IdentityOperation() : base() { } // For use with protobuf-net

        internal IdentityOperation(uint userId, uint sequenceId) : base(userId, sequenceId, 0, (char)0) { }
        public IdentityOperation(DocumentState siteState) : base(siteState, 0, (char)0) { }
        public override string ApplyTransform(string state) => state;
        public override OperationBase CreateInverse(DocumentState documentState) => new IdentityOperation(documentState);
        public override OperationBase NewWithPosition(int newPosition) => new IdentityOperation(UserId, SequenceId);
        public override string ToString()
        {
            return nameof(IdentityOperation);
        }
    }
}