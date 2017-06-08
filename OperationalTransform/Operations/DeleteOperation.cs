using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    public class DeleteOperation : OperationBase
    {
        public static DeleteOperation CreateFromState(int userId, int sequenceId, int position, string state)
        {
            return new DeleteOperation(userId, sequenceId, position, state[position]);
        }
        public DeleteOperation(int userId, int sequenceId, int position, char text) : base(userId, sequenceId, position, text) { }
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
