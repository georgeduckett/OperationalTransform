using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    public abstract class OperationBase
    {
        public readonly int UserId;
        public readonly int SequenceId;
        public readonly int Position;
        public readonly char Text; // If we want this to support strings more than just changing this type is needed, see http://cooffice.ntu.edu.sg/otfaq/ 2.16
        public OperationBase(int userId, int sequenceId, int position, char text)
        {
            UserId = userId;
            SequenceId = sequenceId;
            Position = position;
            Text = text;
        }
        public abstract OperationBase NewWithPosition(int newPosition);
        public int Length => 1;
        public abstract OperationBase CreateInverse();
        public abstract string ApplyTransform(string state);
    }
}
