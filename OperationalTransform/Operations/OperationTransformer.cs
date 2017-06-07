using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    public class OperationTransformer
    {
        /// <summary>
        /// Implements operational transforms as per http://cooffice.ntu.edu.sg/otfaq/ 2.15
        /// </summary>
        /// <param name="remoteOperation">The remote operation we want to transform</param>
        /// <param name="localOperation">The local operation we want to transform using</param>
        /// <returns></returns>
        public OperationBase Transform(OperationBase remoteOperation, OperationBase localOperation)
        {
            switch (remoteOperation)
            {
                case InsertOperation remoteInsert:
                    switch (localOperation)
                    {
                        case InsertOperation localInsert: return TransformInsertInsert(remoteInsert, localInsert);
                        case DeleteOperation localDelete: return TransformInsertDelete(remoteInsert, localDelete);
                        default: throw new InvalidOperationException();
                    }
                case DeleteOperation remoteDelete:
                    switch (localOperation)
                    {
                        case InsertOperation localInsert: return TransformDeleteInsert(remoteDelete, localInsert);
                        case DeleteOperation localDelete: return TransformDeleteDelete(remoteDelete, localDelete);
                        default: throw new InvalidOperationException();
                    }
                default: throw new InvalidOperationException();
            }
        }
        private InsertOperation TransformInsertInsert(InsertOperation remoteInsert, InsertOperation localInsert)
        {
            // TODO: Assert that the state versions that these both are based on is valid (remote is after local)
            if(remoteInsert.Position < localInsert.Position ||
                (remoteInsert.Position == localInsert.Position && remoteInsert.UserId > localInsert.UserId))
            {
                return remoteInsert;
            }
            else
            {
                return new InsertOperation(remoteInsert.UserId, remoteInsert.Position + localInsert.Length, remoteInsert.Text);
            }
        }
        private OperationBase TransformInsertDelete(InsertOperation remoteInsert, DeleteOperation localDelete)
        {
            if(remoteInsert.Position <= localDelete.Position)
            {
                return remoteInsert;
            }
            else
            {
                return new InsertOperation(remoteInsert.UserId, remoteInsert.Position - localDelete.Position, remoteInsert.Text);
            }
        }
        private OperationBase TransformDeleteInsert(DeleteOperation remoteDelete, InsertOperation localInsert)
        {
            if(remoteDelete.Position < localInsert.Position)
            {
                return remoteDelete;
            }
            else
            {
                return new DeleteOperation(remoteDelete.UserId, remoteDelete.Position + localInsert.Length, remoteDelete.Text);
            }
        }
        private OperationBase TransformDeleteDelete(DeleteOperation remoteDelete, DeleteOperation localDelete)
        {
            if(remoteDelete.Position < localDelete.Position)
            {
                return remoteDelete;
            }
            else if(remoteDelete.Position > localDelete.Position)
            {
                return new DeleteOperation(remoteDelete.UserId, remoteDelete.Position - localDelete.Length, remoteDelete.Text);
            }
            else
            {
                return new IdentityOperation(remoteDelete.UserId);
            }
        }

    }
}
