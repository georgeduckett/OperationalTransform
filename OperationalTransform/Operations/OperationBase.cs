using OperationalTransform.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    /// <summary>
    /// Represents an operation to alter a document's text
    /// </summary>
    public abstract class OperationBase
    {
        public readonly uint UserId;
        public readonly uint SequenceId;
        public readonly int Position;
        public readonly char Text; // If we want this to support strings more than just changing this type is needed, see http://cooffice.ntu.edu.sg/otfaq/ 2.16
        public ulong Id => ((ulong)UserId << 32) | SequenceId;
        protected OperationBase(uint userId, uint sequenceId, int position, char text)
        {
            UserId = userId;
            SequenceId = sequenceId;
            Position = position;
            Text = text;
        }
        public OperationBase(DocumentState siteState, int position, char text) : this(siteState.UserId, siteState.GetNextSequenceId(), position, text) { }
        public abstract OperationBase NewWithPosition(int newPosition);
        public int Length => 1;
        public abstract OperationBase CreateInverse();
        public abstract string ApplyTransform(string state);

        public bool IdenticalOperation(OperationBase other)
        {
            if (other == null)
            {
                return false;
            }
            if (GetType() != other.GetType())
            {
                return false;
            }
            return Position == other.Position && Text == other.Text;
        }
        public override string ToString()
        {
            return $"({Position}, '{Text}'), user {UserId}";
        }
    }
}