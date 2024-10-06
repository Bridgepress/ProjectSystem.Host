namespace ProjectSystem.Core.Exceptions
{
    public abstract class ObjectApplicationException : ApplicationException
    {
        public object Object { get; }

        protected ObjectApplicationException(int statusCode, string message, object @object, Exception? inner = null) : base(statusCode,
            message, inner)
        {
            Object = @object;
        }
    }
}
