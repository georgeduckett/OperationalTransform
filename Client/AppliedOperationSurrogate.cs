using OperationalTransform.Operations;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class AppliedOperationSurrogate
    {
        public AppliedOperationSurrogate() { }
        public OperationBase Operation { get; set; }
        public ICollection<ulong> PriorStateTransformIds { get; set; }

        public static implicit operator AppliedOperation(AppliedOperationSurrogate appliedOperationSurrogate)
        {
            var appliedOperation = (AppliedOperation)typeof(AppliedOperation).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null).Invoke(new object[] { });

            typeof(AppliedOperation).GetField(nameof(AppliedOperation.Operation))
                .SetValue(appliedOperation, appliedOperationSurrogate.Operation);
            typeof(AppliedOperation).GetField(nameof(AppliedOperation.PriorStateTransformIds))
                .SetValue(appliedOperation, (IReadOnlyCollection<ulong>)appliedOperationSurrogate.PriorStateTransformIds);

            return appliedOperation;
        }
        public static implicit operator AppliedOperationSurrogate(AppliedOperation appliedOperation)
        {
            if (appliedOperation == null) return null;

            return new AppliedOperationSurrogate()
            {
                Operation = appliedOperation.Operation,
                PriorStateTransformIds = new List<ulong>(appliedOperation.PriorStateTransformIds)
            };
        }
    }
}
