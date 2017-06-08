using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    public class InsertOperation : OperationBase
    {
        public InsertOperation(int userId, int sequenceId, int position, char text) : base(userId, sequenceId, position, text) { }
        public override string ApplyTransform(string state)
        {
            return state.Substring(0, Position) + Text + state.Substring(Position);
        }
        public override OperationBase CreateInverse()
        {
            return new DeleteOperation(UserId, SequenceId, Position, Text);
        }
        public override OperationBase NewWithPosition(int newPosition)
        {
            return new InsertOperation(UserId, SequenceId, newPosition, Text);
        }
    }
}
