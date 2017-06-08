using OperationalTransform.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.StateManagement
{
    public class State
    {
        private HashSet<OperationBase> _OperationBase = new HashSet<OperationBase>();
        public IReadOnlyCollection<OperationBase> _AppliedOperations;
        public string CurrentState { get; set; }
        public State(string initialState)
        {
            CurrentState = initialState;
        }
    }
}
