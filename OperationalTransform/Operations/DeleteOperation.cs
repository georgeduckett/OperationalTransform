using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    public class DeleteOperation : OperationBase
    {
        public DeleteOperation(int userId, int position, char text) : base(userId, position, text) { }
        public override OperationBase CreateInverse()
        {
            return new InsertOperation(UserId, Position, Text);
        }
        public override OperationBase NewWithPosition(int newPosition)
        {
            return new InsertOperation(UserId, newPosition, Text);
        }
        public override string ApplyTransform(string state)
        {
            return state.Substring(0, Position) + state.Substring(Position + Length);
        }
    }
}
