﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalTransform.Operations
{
    public static class OperationTransformer
    {
        /// <summary>
        /// Implements operational transforms as per http://cooffice.ntu.edu.sg/otfaq/ 2.15
        /// It take a remote operation, that was indented to be applied to a document that hadn't yet had the local operation
        /// applied, and translates it to one that maintains the intent when used one a document that did have the local operation applied.
        /// </summary>
        /// <param name="remoteOperation">The remote operation we want to transform</param>
        /// <param name="localOperation">The local operation we want to transform using</param>
        /// <returns></returns>
        public static OperationBase Transform(OperationBase remoteOperation, OperationBase localOperation)
        {
            switch (remoteOperation)
            {
                case InsertOperation remoteInsert:
                    switch (localOperation)
                    {
                        case InsertOperation localInsert: return TransformInsertInsert(remoteInsert, localInsert);
                        case DeleteOperation localDelete: return TransformInsertDelete(remoteInsert, localDelete);
                        case IdentityOperation localIdentity: return remoteOperation;
                        default: throw new InvalidOperationException();
                    }
                case DeleteOperation remoteDelete:
                    switch (localOperation)
                    {
                        case InsertOperation localInsert: return TransformDeleteInsert(remoteDelete, localInsert);
                        case DeleteOperation localDelete: return TransformDeleteDelete(remoteDelete, localDelete);
                        case IdentityOperation localIdentity: return remoteOperation;
                        default: throw new InvalidOperationException();
                    }
                case IdentityOperation remoteIdentity: return remoteOperation;
                default: throw new InvalidOperationException();
            }
        }
        private static OperationBase TransformInsertInsert(InsertOperation remoteInsert, InsertOperation localInsert)
        {
            if(remoteInsert.Position < localInsert.Position ||
                (remoteInsert.Position == localInsert.Position && remoteInsert.UserId > localInsert.UserId))
            {
                return remoteInsert;
            }
            else
            {
                return remoteInsert.NewWithPosition(remoteInsert.Position + localInsert.Length);
            }
        }
        private static OperationBase TransformInsertDelete(InsertOperation remoteInsert, DeleteOperation localDelete)
        {
            if(remoteInsert.Position <= localDelete.Position)
            {
                return remoteInsert;
            }
            else
            {
                return remoteInsert.NewWithPosition(remoteInsert.Position - localDelete.Length);
            }
        }
        private static OperationBase TransformDeleteInsert(DeleteOperation remoteDelete, InsertOperation localInsert)
        {
            if(remoteDelete.Position < localInsert.Position)
            {
                return remoteDelete;
            }
            else
            {
                return remoteDelete.NewWithPosition(remoteDelete.Position + localInsert.Length);
            }
        }
        private static OperationBase TransformDeleteDelete(DeleteOperation remoteDelete, DeleteOperation localDelete)
        {
            if(remoteDelete.Position < localDelete.Position)
            {
                return remoteDelete;
            }
            else if(remoteDelete.Position > localDelete.Position)
            {
                return remoteDelete.NewWithPosition(remoteDelete.Position - localDelete.Length);
            }
            else
            {
                return new IdentityOperation(remoteDelete.UserId, remoteDelete.SequenceId);
            }
        }

    }
}
