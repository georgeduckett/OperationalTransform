using OperationalTransform.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.StateManagement
{
    /// <summary>
    /// Maintains the state of a document.
    /// Documents belong to a user (local copy), have a current state (the text) and a list of previously applied operations
    /// </summary>
    public class DocumentState
    {
        /// <summary>
        /// A collection of all the operations that have been applied to this document
        /// </summary>
        private Dictionary<ulong, OperationBase> _AppliedOperations = new Dictionary<ulong, OperationBase>();
        /// <summary>
        /// A collection of all the operations that have been applied to this document
        /// </summary>
        public IReadOnlyDictionary<ulong, OperationBase> AppliedOperations => _AppliedOperations;
        /// <summary>
        /// The current state of the document
        /// </summary>
        public string CurrentState { get; private set; }
        /// <summary>
        /// The id of the user that this document instance belongs to
        /// </summary>
        public readonly uint UserId;
        /// <summary>
        /// The next sequence id to use for local operations
        /// </summary>
        private uint _NextSequenceId = 0;
        public DocumentState(uint userId, string initialState)
        {
            UserId = userId;
            CurrentState = initialState;
        }
        /// <summary>
        /// Gets the next sequence id to use for a local operation
        /// </summary>
        /// <returns></returns>
        public uint GetNextSequenceId()
        {
            return _NextSequenceId++;
        }

        /// <summary>
        /// Applies an operation based on another site document (defined by a list of ids of operations that were applied prior to this one)
        /// </summary>
        /// <param name="appliedOperation"></param>
        public void ApplyTransform(AppliedOperation appliedOperation)
        {
            var missingRemoteTransformIds = AppliedOperations.Keys.Except(appliedOperation.PriorStateTransformIds);

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