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
        /// The order in which the operations were applied
        /// </summary>
        private List<ulong> _AppliedOperationsOrder = new List<ulong>();
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
            var missingRemoteTransformIds = _AppliedOperationsOrder.Except(appliedOperation.PriorStateTransformIds);

            var operation = appliedOperation.Operation;

            foreach (var id in missingRemoteTransformIds.Reverse())
            {
                operation = OperationTransformer.Transform(operation, _AppliedOperations[id]);
            }

            TransformState(operation);
        }
        /// <summary>
        /// Transforms the document state and records the applied operation
        /// </summary>
        /// <param name="operation"></param>
        private void TransformState(OperationBase operation)
        {
            CurrentState = operation.ApplyTransform(CurrentState);
            _AppliedOperations.Add(operation.Id, operation);
            _AppliedOperationsOrder.Add(operation.Id);
        }
        /// <summary>
        /// Whether we are able to perform an undo
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return _AppliedOperations.Count != 0;
        }
        /// <summary>
        /// Whether we're able to perform a redo
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return false;
        }
        /// <summary>
        /// Undo the last transaction
        /// </summary>
        public AppliedOperation Undo()
        {
            var lastOp = _AppliedOperationsOrder.Select(id => _AppliedOperations[id]).Last(ao => ao.UserId == UserId);

            AppliedOperation undoOperation = new AppliedOperation(lastOp.CreateInverse(this), this);
            ApplyTransform(undoOperation);
            return undoOperation;
        }
        /// <summary>
        /// Redo the last transaction
        /// </summary>
        public AppliedOperation Redo()
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return $"UserId: {UserId}, Applied Operations: {_AppliedOperations.Count}, Content:{(CurrentState.Length > 50 ? CurrentState.Substring(0, 50) + "..." : CurrentState)}";
        }
    }
}