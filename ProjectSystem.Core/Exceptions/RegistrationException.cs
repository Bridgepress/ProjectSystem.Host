using System.Net;

namespace ProjectSystem.Core.Exceptions
{
    public class RegistrationException : ObjectApplicationException
    {
        public RegistrationException(IEnumerable<string> errors, object obj)
            : base((int)HttpStatusCode.BadRequest, $"Errors :{string.Join(",", errors)}", obj)
        {
        }
    }
}
