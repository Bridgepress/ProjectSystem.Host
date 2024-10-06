using System.Net;

namespace ProjectSystem.Core.Exceptions
{
    public class AuthorizationFailedException : ApplicationException
    {
        public AuthorizationFailedException()
            : base((int)HttpStatusCode.Forbidden, "Email or Password is incorrect")
        {
        }
    }
}
