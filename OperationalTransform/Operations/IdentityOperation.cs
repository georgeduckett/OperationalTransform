using OperationalTransform.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    public class IdentityOperation : OperationBase
    {
        internal IdentityOperation(uint userId, uint sequenceId) : base(userId, sequenceId, 0, (char)0) { }
        public IdentityOperation(SiteState siteState) : base(siteState, 0, (char)0) { }
        public override string ApplyTransform(string state) => state;
        public override OperationBase CreateInverse() => this;
        public override OperationBase NewWithPosition(int newPosition) => new IdentityOperation(UserId, SequenceId);
    }
}