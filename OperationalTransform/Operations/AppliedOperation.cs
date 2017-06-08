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
        /// <summary>
        /// A static empty collection to re-use for any applied operation that is applied to a site that's not had any state alterations
        /// </summary>
        private static IReadOnlyCollection<ulong> _EmptyTransformCollection = new ulong[0];
        /// <summary>
        /// The operation that's been applied
        /// </summary>
        public readonly OperationBase Operation;
        /// <summary>
        /// The set of prior operations that were applied to the site prior to this being applied
        /// </summary>
        public readonly IReadOnlyCollection<ulong> PriorStateTransformIds = _EmptyTransformCollection;
        /// <summary>
        /// Create a new applied operation against a site with no prior transformations
        /// </summary>
        /// <param name="operation"></param>
        internal AppliedOperation(OperationBase operation) => Operation = operation;
        /// <summary>
        /// Creates an applied operation based on the operation and a set of prior transformations
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="stateTransformIds"></param>
        internal AppliedOperation(OperationBase operation, IReadOnlyCollection<ulong> stateTransformIds): this(operation) => PriorStateTransformIds = stateTransformIds;
        /// <summary>
        /// Creates an applied operation based on the operation and the set of prior transformations from a site
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="siteState"></param>
        public AppliedOperation(OperationBase operation, DocumentState siteState) : this(operation, (IReadOnlyCollection<ulong>)siteState.AppliedOperations.Keys) { }
    }
}
