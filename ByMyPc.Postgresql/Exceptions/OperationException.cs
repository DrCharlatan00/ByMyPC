using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ByMyPc.Postgresql.Exceptions
{
    public interface OperationException
    {
    }

    public class RemoveOperationException<TCollection> : Exception, OperationException where TCollection : class
    {
        public TCollection collection;

        public RemoveOperationException()
        {
        }

        public RemoveOperationException(string? message) : base(message)
        {
        }

        public RemoveOperationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class UpdateOperationException<TCollection> : Exception, OperationException where TCollection : class
    {
        public TCollection collection;

        public UpdateOperationException()
        {
        }

        public UpdateOperationException(string? message) : base(message)
        {
        }

        public UpdateOperationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class CreateOperationException<TCollection> : Exception, OperationException where TCollection : class
    {
        public TCollection collection;

        public CreateOperationException()
        {
        }

        public CreateOperationException(string? message) : base(message)
        {
        }

        public CreateOperationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
