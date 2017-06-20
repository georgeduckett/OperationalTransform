using OperationalTransform.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    /// <summary>
    /// Represents an operation to add text to a site
    /// </summary>
    public class InsertOperation : OperationBase
    {
        private InsertOperation() : base() { } // For use with protobuf-net

        internal InsertOperation(uint userId, uint sequenceId, int position, char text) : base(userId, sequenceId, position, text) { }
        public InsertOperation(DocumentState siteState, int position, char text) : base(siteState, position, text) { }
        public override string ApplyTransform(string state)
        {
            return state.Substring(0, Position) + Text + state.Substring(Position);
        }
        public override OperationBase CreateInverse(DocumentState documentState)
        {
            return new DeleteOperation(UserId, documentState.GetNextSequenceId(), Position, Text);
        }
        public override OperationBase NewWithPosition(int newPosition)
        {
            return new InsertOperation(UserId, SequenceId, newPosition, Text);
        }
    }
}
