using System.Net;

namespace ProjectSystem.Core.Exceptions
{
    public class NotFoundUserByUserNameException : ApplicationException
    {
        public NotFoundUserByUserNameException(string userName)
        : base((int)HttpStatusCode.NotFound, $"User with user name {userName} not found")
        {
        }
    }
}
