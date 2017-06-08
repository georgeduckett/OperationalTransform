using OperationalTransform.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    /// <summary>
    /// Represents an operation to delete text from a site
    /// </summary>
    public class DeleteOperation : OperationBase
    {
        internal DeleteOperation(uint userId, uint sequenceId, int position, char text) : base(userId, sequenceId, position, text) { }
        /// <summary>
        /// Creates a delete operation based for use on the specified site
        /// </summary>
        /// <param name="siteState"></param>
        /// <param name="position"></param>
        public DeleteOperation(DocumentState siteState, int position) : base(siteState, position, siteState.CurrentState[position]) { }
        public override OperationBase CreateInverse()
        {
            return new InsertOperation(UserId, SequenceId, Position, Text);
        }
        public override OperationBase NewWithPosition(int newPosition)
        {
            return new DeleteOperation(UserId, SequenceId, newPosition, Text);
        }
        public override string ApplyTransform(string state)
        {
            return state.Substring(0, Position) + state.Substring(Position + Length);
        }
    }
}
