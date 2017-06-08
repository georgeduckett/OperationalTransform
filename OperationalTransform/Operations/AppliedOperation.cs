using OperationalTransform.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    /// <summary>
    /// A combination of an operation and the state (defined by the set of previously applied operations) it was applied to
    /// </summary>
    public class AppliedOperation
    {
        private static IReadOnlyCollection<ulong> _EmptyCollection = (IReadOnlyCollection<ulong>)Enumerable.Empty<ulong>();
        public readonly OperationBase Operation;
        public readonly IReadOnlyCollection<ulong> StateTransformIds = _EmptyCollection;
        public AppliedOperation(OperationBase operation) => Operation = operation;
        public AppliedOperation(OperationBase operation, IReadOnlyCollection<ulong> stateTransformIds): this(operation) => StateTransformIds = stateTransformIds;
        public AppliedOperation(OperationBase operation, SiteState siteState) : this(operation, (IReadOnlyCollection<ulong>)siteState.AppliedOperations.Keys) { }
        public AppliedOperation(OperationBase operation, params ulong[] stateTransformIds) :this(operation) => StateTransformIds = stateTransformIds;
    }
}
