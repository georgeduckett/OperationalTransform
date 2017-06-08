using OperationalTransform.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.StateManagement
{
    /// <summary>
    /// Maintains the state of a document at a user's site
    /// </summary>
    public class SiteState
    {
        private Dictionary<ulong, OperationBase> _AppliedOperations = new Dictionary<ulong, OperationBase>();
        public IReadOnlyDictionary<ulong, OperationBase> AppliedOperations => _AppliedOperations;
        public string CurrentState { get; private set; }
        public readonly uint UserId;
        public uint _NextSequenceId = 0;
        public SiteState(uint userId, string initialState)
        {
            UserId = userId;
            CurrentState = initialState;
        }

        public uint GetNextSequenceId()
        {
            return _NextSequenceId++;
        }

        /// <summary>
        /// Applies an operation based on another site state (defined by a list of applied operation ids)
        /// </summary>
        /// <param name="appliedOperation"></param>
        public void ApplyTransform(AppliedOperation appliedOperation)
        {
            var missingRemoteTransformIds = AppliedOperations.Keys.Except(appliedOperation.StateTransformIds);

            var operation = appliedOperation.Operation;

            foreach(var id in missingRemoteTransformIds)
            {
                operation = OperationTransformer.Transform(operation, _AppliedOperations[id]);
            }

            CurrentState = operation.ApplyTransform(CurrentState);
            _AppliedOperations.Add(operation.Id, operation);
        }
    }
}