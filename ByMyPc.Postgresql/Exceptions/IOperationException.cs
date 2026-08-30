using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ByMyPc.Postgresql.Exceptions
{
    public interface IOperationException
    {
        Type? CollectionThrow { get; }
        string NameCollection { get; }
    }

    public class RemoveOperationException<TCollection> : Exception, IOperationException where TCollection : class
    {
        public TCollection collection;

        public Type? CollectionThrow { get; }
        public string NameCollection { get; }

        public RemoveOperationException()
        {
            CollectionThrow = collection?.GetType();
            NameCollection = collection?.GetType()?.FullName ?? "Collection name unknown";

        }

        public RemoveOperationException(string? message) : base(message)
        {
            CollectionThrow = collection?.GetType();
            NameCollection = collection?.GetType()?.FullName ?? "Collection name unknown";
        }

        public RemoveOperationException(string? message, Exception? innerException) : base(message, innerException)
        {
            CollectionThrow = collection?.GetType();
            NameCollection = collection?.GetType()?.FullName ?? "Collection name unknown";
        }
    }

    public class UpdateOperationException<TCollection> : Exception, IOperationException where TCollection : class
    {
        public Type? CollectionThrow { get; }
        public string NameCollection { get; }
        public TCollection collection;

        public UpdateOperationException()
        {
            CollectionThrow = collection?.GetType();
            NameCollection = collection?.GetType()?.FullName ?? "Collection name unknown";
        }

        public UpdateOperationException(string? message) : base(message)
        {
            CollectionThrow = collection?.GetType();
            NameCollection = collection?.GetType()?.FullName ?? "Collection name unknown";
        }

        public UpdateOperationException(string? message, Exception? innerException) : base(message, innerException)
        {
            CollectionThrow = collection?.GetType();
            NameCollection = collection?.GetType()?.FullName ?? "Collection name unknown";
        }
    }

    public class CreateOperationException<TCollection> : Exception, IOperationException where TCollection : class
    {
        public Type? CollectionThrow { get; }
        public string NameCollection { get; }
        public TCollection collection;

        public CreateOperationException()
        {
            CollectionThrow = collection?.GetType();
            NameCollection = collection?.GetType()?.FullName ?? "Collection name unknown";
        }

        public CreateOperationException(string? message) : base(message)
        {
            CollectionThrow = collection?.GetType();
            NameCollection = collection?.GetType()?.FullName ?? "Collection name unknown";
        }

        public CreateOperationException(string? message, Exception? innerException) : base(message, innerException)
        {
            CollectionThrow = collection?.GetType();
            NameCollection = collection?.GetType()?.FullName ?? "Collection name unknown";
        }
    }
}
