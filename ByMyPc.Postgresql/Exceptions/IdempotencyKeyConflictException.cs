namespace ByMyPc.Postgresql.Exceptions
{
    [Serializable]
    public class IdempotencyKeyConflictException : Exception
    {
        public IdempotencyKeyConflictException()
        {
        }

        public IdempotencyKeyConflictException(string? message) : base(message)
        {
        }

        public IdempotencyKeyConflictException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}