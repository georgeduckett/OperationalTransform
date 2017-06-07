using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    public class IdentityOperation : OperationBase
    {
        public IdentityOperation(int userId) : base(userId, 0, (char)0) { }
        public override string ApplyTransform(string state) => state;
        public override OperationBase CreateInverse() => this;
        public override OperationBase NewWithPosition(int newPosition) => this;
    }
}
